using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Topics.Radical.Reflection;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Radical.Bootstrapper.Windsor.WebAPI.Installers
{
    [Export(typeof(IWindsorInstaller))]
    public sealed class WebApiDefaultInstaller : IWindsorInstaller
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

            container.Register
            (
                Types.FromAssemblyInDirectory(new AssemblyFilter(directory, filter))
                    .IncludeNonPublicTypes()
                    .Where(t => t.Namespace != null && !t.IsAbstract && !t.IsInterface && t.Is<IFilter>())
                    .WithService.AllInterfaces()
                    .LifestyleTransient()
            );

            container.Register
            (
                Types.FromAssemblyInDirectory(new AssemblyFilter(directory, filter))
                                .BasedOn<IHttpController>()
                                .LifestyleTransient()
            );
        }
    }
}
