using Divergent.Customers.Data.Context;
using Divergent.ITOps.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Divergent.Customers.Data;

public class ServiceRegistration : IRegisterServices
{
    public void Register(HostBuilderContext context, IServiceCollection services)
    {
        services.AddDbContext<CustomersContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("Divergent.Customers")));

    }
}