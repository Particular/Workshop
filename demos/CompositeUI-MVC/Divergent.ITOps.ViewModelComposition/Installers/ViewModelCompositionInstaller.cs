using Castle.MicroKernel.Registration;
using System.ComponentModel.Composition;
using Castle.Windsor;
using Castle.MicroKernel.SubSystems.Configuration;
using Topics.Radical.Reflection;
using System.Web.Mvc;
using Radical.Bootstrapper;
using Topics.Radical.ComponentModel.Messaging;
using Topics.Radical.Messaging;
using Topics.Radical.Threading;

namespace Divergent.ITOps.ViewModelComposition.Installers
{
    /// <summary>
    /// Default boot installer.
    /// </summary>
    [Export(typeof(IWindsorInstaller))]
    public sealed class ViewModelCompositionInstaller : IWindsorInstaller
    {
        /// <summary>
        /// Performs the installation in the <see cref="T:Castle.Windsor.IWindsorContainer"/>.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="store">The configuration store.</param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var boot = container.Resolve<IBootstrapper>();
            var directory = boot.ProbeDirectory;
            var filter = boot.AssemblyFilter;

            container.Register(Component.For<IMessageBroker>().Instance(new MessageBroker(new NullDispatcher())));

            container.Register
            (
                Types.FromAssemblyInDirectory(new AssemblyFilter(directory, filter))
                    .IncludeNonPublicTypes()
                    .Where(t => t.Namespace != null && !t.IsAbstract && !t.IsInterface && t.Is<IViewModelAppender>())
                    .WithService.FirstInterface()
                    .LifestyleSingleton()
            );

            container.Register
            (
                Types.FromAssemblyInDirectory(new AssemblyFilter(directory, filter))
                    .IncludeNonPublicTypes()
                    .Where(t => t.Namespace != null && !t.IsAbstract && !t.IsInterface && t.Is<ISubscribeToCompositionEvents>())
                    .WithService.FirstInterface()
                    .LifestyleSingleton()
            );
        }
    }
}
