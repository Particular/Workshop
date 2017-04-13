using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITOps.ViewModelComposition.Gateway
{
    public interface IRegisterRoutes
    {
        void RegisterRoutes(RouteBuilder routeBuilder, RouteHandler defaultGatewayRouteHandler);
    }
}
