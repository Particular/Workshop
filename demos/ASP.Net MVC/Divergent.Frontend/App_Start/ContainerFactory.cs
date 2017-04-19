using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Divergent.ITOps.ViewModelComposition;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace Divergent.Frontend
{
    public static class ContainerFactory
    {
        public static IWindsorContainer Create()
        {
            var container = new WindsorContainer();
            container.Kernel.Resolver.AddSubResolver(new ArrayResolver(container.Kernel, true));

            container.Register(Component.For<IResultFilter>()
                    .ImplementedBy<CompositionActionFilter>()
                    .LifestyleTransient());

            var bin = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
            var concreteTypes = Directory.EnumerateFiles(bin, "Divergent*.dll")
                .SelectMany(path => Assembly.LoadFile(path).GetTypes())
                .Where(type => !type.IsInterface && !type.IsAbstract);

            foreach (var type in concreteTypes.Where(t => typeof(IRouteInterceptor).IsAssignableFrom(t)))
            {
                if (typeof(ISubscribeToCompositionEvents).IsAssignableFrom(type))
                {
                    container.Register(Component.For<ISubscribeToCompositionEvents, IRouteInterceptor>()
                        .ImplementedBy(type)
                        .LifestyleSingleton());
                }

                if (typeof(IViewModelAppender).IsAssignableFrom(type))
                {
                    container.Register(Component.For<IViewModelAppender, IRouteInterceptor>()
                        .ImplementedBy(type)
                        .LifestyleSingleton());
                }
            }

            return container;
        }
    }
}
