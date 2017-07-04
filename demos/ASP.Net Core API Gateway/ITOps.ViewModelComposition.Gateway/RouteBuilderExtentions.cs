using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.DependencyInjection;

namespace ITOps.ViewModelComposition.Gateway
{
    public static class RouteBuilderExtentions
    {
        public static IRouteBuilder MapComposableGet(this IRouteBuilder routeBuilder,
           string template,
           RouteValueDictionary defaults = null,
           RouteValueDictionary dataTokens = null)
        {
            var route = new Route(
                target: new RouteHandler(ctx => ComposableRouteHandler.HandleGetRequest(ctx)),
                routeTemplate: template,
                defaults: defaults,
                constraints: new RouteValueDictionary(new
                {
                    httpMethod = new HttpMethodRouteConstraint(HttpMethods.Get)
                }),
                dataTokens: dataTokens,
                inlineConstraintResolver: routeBuilder.ServiceProvider.GetRequiredService<IInlineConstraintResolver>()
            );

            routeBuilder.Routes.Add(route);

            return routeBuilder;
        }
    }
}
