## ASP.NET Core demo

This demo is composed of two solutions which demonstrate UI Composition techniques using ASP.NET Core (v1.1.2).

### Divergent.CompositionGateway

`Divergent.CompositionGateway` shows how to host an ASP.NET Core API gateway, or reverse proxy, that composes HTTP responses from multiple backend APIs. To run this sample ensure that the following projects are set as startup projects:

* `Divergent.Sales.API.Host`
* `Divergent.Shipping.API.Host`
* `Divergent.CompositionGateway`

Use an HTTP client such as [Postman](https://chrome.google.com/webstore/detail/postman/fhbjgbiflinjbdggehcddcbncdddomop?hl=en) to test the gateway.

### Divergent.Website

`Divergent.Website` is an ASP.NET Core MVC app that composes HTTP responses from multiple backend APIs directly into MVC views.  To run this sample ensure that the following projects are set as startup projects:

* `Divergent.Sales.API.Host`
* `Divergent.Shipping.API.Host`
* `Divergent.Website`

Use a web browser to test the website.
