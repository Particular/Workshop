using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
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
            var container = new WindsorContainer();
            container.Kernel.Resolver.AddSubResolver(new ArrayResolver(container.Kernel, true));

            var bin = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
            var concreteTypes = Directory.EnumerateFiles(bin, "Divergent*.dll")
                .SelectMany(path => Assembly.LoadFile(path).GetTypes())
                .Where(t => !t.IsInterface && !t.IsAbstract);

            container.Register(Component.For<IResultFilter>()
                    .ImplementedBy<CompositionActionFilter>()
                    .LifestyleTransient());

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

            return container;
        }
    }
}
