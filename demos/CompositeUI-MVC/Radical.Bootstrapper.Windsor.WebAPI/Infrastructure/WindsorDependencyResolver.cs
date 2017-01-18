using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using Castle.MicroKernel.Lifestyle;

namespace Radical.Bootstrapper.Windsor.WebAPI.Infrastructure
{
    public class WindsorDependencyResolver : System.Web.Http.Dependencies.IDependencyResolver
    {
        readonly IWindsorContainer container;

        public WindsorDependencyResolver( IWindsorContainer container )
        {
            this.container = container;
        }

        public Func<IWindsorContainer, IDependencyScope> OnBeginScope { get; set; }

        public IDependencyScope BeginScope()
        {
			if( this.OnBeginScope != null)
            {
                var scope = this.OnBeginScope(this.container);

				return scope;
            }

			var ws = this.container.BeginScope();
			return new WindsorDependencyScope(this.container, ws);
        }

        public object GetService( Type serviceType )
        {
            if ( this.container.Kernel.HasComponent( serviceType ) )
            {
                return this.container.Resolve( serviceType );
            }

            return null;
        }

        public IEnumerable<object> GetServices( Type serviceType )
        {
            if ( this.container.Kernel.HasComponent( serviceType ) )
            {
                var all = this.container.ResolveAll( serviceType );
                return all.Cast<Object>();
            }

            return new List<Object>();
        }

        public void Dispose()
        {

        }
    }
}
