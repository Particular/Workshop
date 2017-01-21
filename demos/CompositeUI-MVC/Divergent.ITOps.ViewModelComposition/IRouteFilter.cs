﻿using System.Web.Routing;

namespace Divergent.ITOps.ViewModelComposition
{
    public interface IRouteFilter
    {
        bool Matches(RouteData routeData);
    }
}