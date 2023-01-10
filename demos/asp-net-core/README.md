## ASP.NET Core demo

This demo is composed of two solutions which demonstrate UI Composition techniques using ASP.NET Core.

In order to run the demos, you require the latest .NET Core Runtime in the [7.0.x version range](https://www.microsoft.com/net/download/all).

### Divergent.CompositionGateway

`Divergent.CompositionGateway` shows how to host an ASP.NET Core API gateway, or reverse proxy, that composes HTTP responses from multiple backend APIs. To run this sample ensure that the following projects are set as startup projects:

* `Divergent.Sales.API`
* `Divergent.Shipping.API`
* `Divergent.CompositionGateway`

Use a browser or an HTTP client such as [REST Client for VS Code](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) to test the gateway. Sample HTTP requests are available in the [dsemo-requests-list.rest](dsemo-requests-list.rest) document.

### Divergent.Website

`Divergent.Website` is an ASP.NET Core MVC app that composes HTTP responses from multiple backend APIs directly into MVC views.  To run this sample ensure that the following projects are set as startup projects:

* `Divergent.Sales.API`
* `Divergent.Shipping.API`
* `Divergent.Website`

Use a web browser to test the website.
