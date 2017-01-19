using Castle.MicroKernel.Registration;
using System.ComponentModel.Composition;
using Castle.Windsor;
using Castle.MicroKernel.SubSystems.Configuration;
using Topics.Radical.Reflection;
using System.Web.Mvc;
using Radical.Bootstrapper.Windsor.Mvc.Infrastructure;

namespace Radical.Bootstrapper.Windsor.Mvc.Installers
{
    /// <summary>
    /// Default boot installer.
    /// </summary>
    [Export( typeof( IWindsorInstaller ) )]
	public sealed class MvcDefaultInstaller : IWindsorInstaller
	{
		/// <summary>
		/// Performs the installation in the <see cref="T:Castle.Windsor.IWindsorContainer"/>.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <param name="store">The configuration store.</param>
		public void Install( IWindsorContainer container, IConfigurationStore store )
		{
			var boot = container.Resolve<IBootstrapper>();
			var directory = boot.ProbeDirectory;
            var filter = boot.AssemblyFilter;

			container.Register
			(
				Component.For<IControllerFactory>()
					.ImplementedBy<WindsorControllerFactory>()
                    .IsFallback()
			);

			container.Register
			(
				Component.For<IActionInvoker>()
					.ImplementedBy<WindsorActionInvoker>()
                    .IsFallback()
			);

			container.Register
			(
				Types.FromAssemblyInDirectory( new AssemblyFilter( directory, filter ) )
                    .IncludeNonPublicTypes()
                    .BasedOn<IController>()
					.LifestyleTransient()
			);

			container.Register
			(
                Types.FromAssemblyInDirectory( new AssemblyFilter( directory, filter ) )
					.IncludeNonPublicTypes()
					.Where( t => t.Namespace != null && !t.IsAbstract && !t.IsInterface && t.Is<IAuthorizationFilter>() )
					.WithService.FirstInterface()
					.LifestyleTransient()
			);

			container.Register
			(
                Types.FromAssemblyInDirectory( new AssemblyFilter( directory, filter ) )
					.IncludeNonPublicTypes()
					.Where( t => t.Namespace != null && !t.IsAbstract && !t.IsInterface && t.Is<IActionFilter>() )
					.WithService.FirstInterface()
					.LifestyleTransient()
			);

			container.Register
			(
                Types.FromAssemblyInDirectory( new AssemblyFilter( directory, filter ) )
					.IncludeNonPublicTypes()
					.Where( t => t.Namespace != null && !t.IsAbstract && !t.IsInterface && t.Is<IResultFilter>() )
					.WithService.FirstInterface()
					.LifestyleTransient()
			);

			container.Register
			(
                Types.FromAssemblyInDirectory( new AssemblyFilter( directory, filter ) )
					.IncludeNonPublicTypes()
					.Where( t => t.Namespace != null && !t.IsAbstract && !t.IsInterface && t.Is<IExceptionFilter>() )
					.WithService.FirstInterface()
					.LifestyleTransient()
			);
		}
	}
}
