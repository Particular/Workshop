using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Divergent.ITOps.ViewModelComposition;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace Divergent.Frontend
{
    public class ContainerConfig
    {
        public static IWindsorContainer ConfigContainer()
        {
            var bin = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
            var concreteTypes = Directory.EnumerateFiles(bin, "Divergent.*")
                .SelectMany(path => Assembly.LoadFile(path).GetTypes())
                .Where(t => !t.IsInterface);

            var container = new WindsorContainer();
            foreach (var t in concreteTypes.Where(t => typeof(IResultFilter).IsAssignableFrom(t)))
            {
                container.Register(Component.For<IResultFilter>()
                    .ImplementedBy(t)
                    .LifestyleTransient());
            }

            foreach (var t in concreteTypes.Where(t => typeof(IRouteInterceptor).IsAssignableFrom(t)))
            {
                if (typeof(ISubscribeToCompositionEvents).IsAssignableFrom(t))
                {
                    container.Register(Component.For<ISubscribeToCompositionEvents, IRouteInterceptor>()
                        .ImplementedBy(t)
                        .LifestyleSingleton());
                }

                if (typeof(IViewModelAppender).IsAssignableFrom(t))
                {
                    container.Register(Component.For<IViewModelAppender, IRouteInterceptor>()
                        .ImplementedBy(t)
                        .LifestyleSingleton());
                }
            }

        }
    }
}
