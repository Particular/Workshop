using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Divergent.ITOps.Interfaces
{
    public interface IRegisterServices
    {
        void Register(HostBuilderContext context, IServiceCollection services);
    }
}