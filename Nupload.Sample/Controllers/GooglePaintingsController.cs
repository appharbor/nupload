using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Nupload.Sample.Models;

namespace Nupload.Sample.Controllers
{
	public class GooglePaintingsController : Controller
	{
		private readonly DatabaseContext _databaseContext;
		private readonly GoogleCredentials _credentials;

		public GooglePaintingsController()
		{
			_databaseContext = new DatabaseContext();

			using (var certificateStream = GetEmbeddedResourceStream<MvcApplication>("google-cloud-storage-privatekey.p12"))
			{
				_credentials = new GoogleCredentials("123456789@developer.gserviceaccount.com", certificateStream, "foobarbaz");
			}
		}

		private Stream GetEmbeddedResourceStream<T>(string resourceName)
		{
			var type = typeof(T);
			var assembly = type.Assembly;

			return assembly.GetManifestResourceStream(type, resourceName);
		}

		public ViewResult Index()
		{
			return View("~/Views/Paintings/Index.cshtml", _databaseContext.Paintings.ToList());
		}

		public ActionResult Details(int id)
		{
			var painting = _databaseContext.Paintings.Find(id);
			var urlSigner = new GoogleCloudStorageUrlSigner(_credentials);
			var signedUrl = urlSigner.GetSignedUrl(new Uri(painting.ImageUrl), TimeSpan.FromMinutes(20));

			return Json(new
			{
				id = painting.Id,
				signedUrl = signedUrl,
				name = painting.Name
			},
			JsonRequestBehavior.AllowGet);
		}

		public ActionResult Create()
		{
			var maxFileSize = 512 * 1024 * 1024;
			var randomStringGenerator = new RandomStringGenerator();
			var objectKey = string.Format("uploads/{0}/${{filename}}", randomStringGenerator.GenerateString(16));

			var objectConfiguration = new GoogleCloudStorageObjectConfiguration(objectKey, GoogleCloudStorageCannedAcl.Private, maxFileSize);
			var uploadConfiguration = new GoogleCloudStorageUploadConfiguration(_credentials, "foobucket", objectConfiguration);

			return View("~/Views/Paintings/Create.cshtml", uploadConfiguration);
		}

		[HttpPost]
		public ActionResult Create(Painting painting)
		{
			if (ModelState.IsValid)
			{
				var imageUri = new Uri(painting.ImageUrl);
				painting.Name = Server.UrlDecode(imageUri.Segments.Last());

				_databaseContext.Paintings.Add(painting);
				_databaseContext.SaveChanges();

				Response.RedirectLocation = Url.Action("details", new { id = painting.Id });
				return new HttpStatusCodeResult((int)HttpStatusCode.Created);
			}

			return View(painting);
		}

		protected override void Dispose(bool disposing)
		{
			_databaseContext.Dispose();
			base.Dispose(disposing);
		}
	}
}
