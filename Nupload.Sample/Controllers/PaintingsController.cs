using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Nupload.Sample.Models;

namespace Nupload.Sample.Controllers
{
	public class PaintingsController : Controller
	{
		private readonly AmazonCredentials _credentials;
		private readonly DatabaseContext _databaseContext;

		public PaintingsController()
		{
			_credentials = new AmazonCredentials(ConfigurationManager.AppSettings.Get("amazon.access_key_id"),
				ConfigurationManager.AppSettings.Get("amazon.secret_access_key"));
			_databaseContext = new DatabaseContext();
		}

		public ViewResult Index()
		{
			var paintings = _databaseContext.Paintings.ToList();

			return View(paintings);
		}

		public ActionResult Details(int id)
		{
			Painting painting = _databaseContext.Paintings.Find(id);
			var urlSigner = new AmazonS3UrlSigner(_credentials);
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
			var randomStringGenerator = new RandomStringGenerator();
			var objectKey = string.Format("uploads/{0}/${{filename}}", randomStringGenerator.GenerateString(16));
			var bucket = "foo-bucket";

			var maxFileSize = 512 * 1024 * 1024;
			var objectConfiguration = new AmazonS3ObjectConfiguration(objectKey, AmazonS3CannedAcl.Private, maxFileSize);
			var uploadConfiguration = new AmazonS3UploadConfiguration(_credentials, bucket, objectConfiguration);

			return View(uploadConfiguration);
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