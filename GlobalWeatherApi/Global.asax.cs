using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;

namespace GlobalWeatherApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static IWindsorContainer _container;
        protected void Application_Start()
        {
            SetupContainer(GlobalConfiguration.Configuration);

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_End()
        {
            _container?.Dispose();
            Dispose();
        }

        private static void SetupContainer(HttpConfiguration configuration)
        {
            _container = new WindsorContainer();
            _container.Install(new WeatherApiInstaller());
            _container.Kernel.Resolver.AddSubResolver(new CollectionResolver(_container.Kernel, true));
            configuration.DependencyResolver = new DependencyResolver(_container.Kernel);
        }
    }
}
