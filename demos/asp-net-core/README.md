## ASP.NET Core demo

This demo is composed of two solutions which demonstrate UI Composition techniques using ASP.NET Core.

In order to run the demos, you require the latest .NET Core Runtime in the [2.0.x version range](https://www.microsoft.com/net/download/all).

### Divergent.CompositionGateway

`Divergent.CompositionGateway` shows how to host an ASP.NET Core API gateway, or reverse proxy, that composes HTTP responses from multiple backend APIs. To run this sample ensure that the following projects are set as startup projects:

* `Divergent.Sales.API.Host`
* `Divergent.Shipping.API.Host`
* `Divergent.CompositionGateway`

Use an HTTP client such as [Postman](https://chrome.google.com/webstore/detail/postman/fhbjgbiflinjbdggehcddcbncdddomop?hl=en) to test the gateway. Here are a few sample HTTP requests that can be used:

* Order #1, Sales only: http://localhost:20295/api/orders/1 - gets order 1 details from the Sales endpoint
* Order #1, Shipping only: http://localhost:20296/api/shipments/order/1 - gets order 1 details from the Shipping endpoint
* Order #1, composed: http://localhost:4457/orders/1 - gets order 1 from the composition gateway showing details composed from both Sales and Shipping
* Orders list, Sales only: http://localhost:20295/api/orders?pageSize=10&pageIndex=0 - gets a list of order details from the Sales endpoint
* Orders list, composed: http://localhost:4457/orders - gets a list of orders from the composition gateway showing details composed from both Sales and Shipping

The [postman-collection.json](postman-collection.json) file contains the same set of HTTP requests as a collection that can be imported directly into Postman.

### Divergent.Website

`Divergent.Website` is an ASP.NET Core MVC app that composes HTTP responses from multiple backend APIs directly into MVC views.  To run this sample ensure that the following projects are set as startup projects:

* `Divergent.Sales.API.Host`
* `Divergent.Shipping.API.Host`
* `Divergent.Website`

Use a web browser to test the website.
