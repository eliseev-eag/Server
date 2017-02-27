using Ninject;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi;
using Ninject.Web.WebApi.OwinHost;
using Owin;
using System;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Http;

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

            config.DependencyResolver = new NinjectDependencyResolver(kernel);

            appBuilder.UseNinjectMiddleware(() => kernel).UseNinjectWebApi(config);
            appBuilder.UseWebApi(config);
        }
        /*
        private static Lazy<IKernel> CreateKernel = new Lazy<IKernel>(() =>
        {
            StandardKernel kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            return kernel;
        });*/
    }
}
