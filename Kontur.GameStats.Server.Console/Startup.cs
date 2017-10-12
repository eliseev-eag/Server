using Microsoft.Owin;
using Ninject;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.OwinHost;
using Owin;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Http;

[assembly: OwinStartup(typeof(Kontur.GameStats.Server.Startup))]

namespace Kontur.GameStats.Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();

            config.Formatters.JsonFormatter.SupportedMediaTypes
                    .Add(new MediaTypeHeaderValue("text/html"));

            StandardKernel kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            
            appBuilder.UseNinjectMiddleware(() => kernel).UseNinjectWebApi(config);
            appBuilder.UseWebApi(config);
        }
    }
}
