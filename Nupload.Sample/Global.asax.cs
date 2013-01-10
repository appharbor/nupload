using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nupload.Sample
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			RegisterRoutes(RouteTable.Routes);
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				"Default",
				"{controller}/{action}/{id}",
				new { controller = "Paintings", action = "Index", id = UrlParameter.Optional }
			);

		}
	}
}
