using Castle.MicroKernel.Registration;
using System.ComponentModel.Composition;
using Castle.Windsor;
using Castle.MicroKernel.SubSystems.Configuration;
using Topics.Radical.Reflection;
using System.Web.Mvc;
using Radical.Bootstrapper;

namespace Divergent.ITOps.ViewModelComposition.Installers
{
    /// <summary>
    /// Default boot installer.
    /// </summary>
    [Export( typeof( IWindsorInstaller ) )]
	public sealed class AppendersInstaller : IWindsorInstaller
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
				Types.FromAssemblyInDirectory( new AssemblyFilter( directory, filter ) )
                    .IncludeNonPublicTypes()
                    .BasedOn<IViewModelAppender>()
					.LifestyleTransient()
			);
		}
	}
}
