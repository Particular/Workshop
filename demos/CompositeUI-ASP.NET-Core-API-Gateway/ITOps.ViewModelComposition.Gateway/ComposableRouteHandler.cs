using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITOps.ViewModelComposition.Gateway
{
    public class ComposableRouteHandler
    {
        public static async Task HandleGetRequest(HttpContext context)
        {
            var result = await CompositionHandler.HandleGetRequest(context);
            if (result.StatusCode == StatusCodes.Status200OK)
            {
                string json = JsonConvert.SerializeObject(result.ViewModel, GetSettings(context));
                await context.Response.WriteAsync(json);
            }
            else
            {
                context.Response.StatusCode = result.StatusCode;
            }
        }

        static JsonSerializerSettings GetSettings(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("Accept-Casing", out StringValues casing))
            {
                casing = "casing/camel";
            }

            switch (casing)
            {
                case "casing/pascal":
                    return new JsonSerializerSettings();

                default: // "casing/camel":
                    return new JsonSerializerSettings()
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    };
            }
        }
    }
}
