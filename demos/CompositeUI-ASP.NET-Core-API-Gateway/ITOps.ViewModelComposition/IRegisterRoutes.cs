using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITOps.ViewModelComposition
{
    public interface IRegisterRoutes
    {
        void RegisterRoutes(IRouteBuilder routeBuilder, IRouteHandler gatewayRouteHandler);
    }
}
