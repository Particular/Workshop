using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;

namespace ITOps.ViewModelComposition.Gateway
{
    class ComposableRouteHandler
    {
        public static async Task HandleGetRequest(HttpContext context)
        {
            var result = await CompositionHandler.HandleGetRequest(context);

            if (result.StatusCode == StatusCodes.Status200OK)
            {
                // For the purposes of the demo, we're not respecting the HTTP Accept header and assuming that a JSON response is OK.
                context.Response.ContentType = "application/json; charset=utf-8";
                string json = JsonConvert.SerializeObject(result.ViewModel, GetSettings(context));
                await context.Response.WriteAsync(json);
            }
            else
            {
                context.Response.StatusCode = result.StatusCode;
            }
        }

        private static JsonSerializerSettings GetSettings(HttpContext context)
        {
            context.Request.Headers.TryGetValue("Accept-Casing", out var casing);

            switch (casing)
            {
                case "casing/pascal":
                    return new JsonSerializerSettings();

                default: // camelCase
                    return new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    };
            }
        }
    }
}
