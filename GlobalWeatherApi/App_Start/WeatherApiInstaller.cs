using System.Web.Http;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using GlobalWeatherApi.BusinessLogic;

namespace GlobalWeatherApi
{
    public class WeatherApiInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                //controllers
                Classes.FromThisAssembly().BasedOn<ApiController>().LifestyleScoped(),

                //business logic
                Component.For<IWeatherServiceClient>().ImplementedBy<WeatherServiceClient>().LifestyleTransient(),
                Component.For<IHttpClientFactory>().ImplementedBy<HttpClientFactory>().LifestyleSingleton(),
                Component.For<IDataFileReader>().ImplementedBy<DataFileReader>().LifestyleTransient(),
                Component.For<IXmlHelper>().ImplementedBy<XmlHelper>().LifestyleSingleton(),
                Component.For<IExecutionContext>().ImplementedBy<ExecutionContext>().LifestyleSingleton()
                );
        }
    }
}