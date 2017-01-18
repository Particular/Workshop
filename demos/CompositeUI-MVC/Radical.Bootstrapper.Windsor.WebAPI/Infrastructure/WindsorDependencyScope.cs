using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;

namespace Radical.Bootstrapper.Windsor.WebAPI.Infrastructure
{
    class WindsorDependencyScope : IDependencyScope
	{
		private Castle.Windsor.IWindsorContainer windsorContainer;
		private IDisposable ws;

		public WindsorDependencyScope(Castle.Windsor.IWindsorContainer windsorContainer, IDisposable ws)
		{
			// TODO: Complete member initialization
			this.windsorContainer = windsorContainer;
			this.ws = ws;
		}
		public object GetService(Type serviceType)
		{
			if (this.windsorContainer.Kernel.HasComponent(serviceType))
			{
				return this.windsorContainer.Resolve(serviceType);
			}

			return null;
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			if (this.windsorContainer.Kernel.HasComponent(serviceType))
			{
				return this.windsorContainer.ResolveAll(serviceType)
					.OfType<Object>();
			}

			return new Object[0];
		}

		public void Dispose()
		{
			this.ws.Dispose();
		}
	}
}
