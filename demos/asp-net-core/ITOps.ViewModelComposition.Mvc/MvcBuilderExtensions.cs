using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ITOps.ViewModelComposition.Mvc
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddViewModelCompositionMvcSupport(this IMvcBuilder builder)
        {
            builder.Services.Configure<MvcOptions>(options => 
            {
                options.Filters.Add(typeof(CompositionFilter));
            });

            return builder;
        }
    }
}
