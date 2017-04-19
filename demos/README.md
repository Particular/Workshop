# Demos

This folder contains all the demos available during the Microservices Workshop

## ASP.Net MVC - demo

The `ASP.Net MVC` solution demoes UI Composition techniques when using regular ASP.Net MVC 5. To run the demo ensure that the following projects are set as startup projects:

* `Divergent.Sales.API.Host`
* `Divergent.Shipping.API.Host`
* `Divergent.Frontend`

## ASP.Net Core API Gateway - demo

The `ASP.Net Core API Gateway` solution demoes UI Composition techniques built on top of .Net Core (v1.1.2). The demo is composed by 3 different main samples:

### Divergent.CompositionGateway - sample

`Divergent.CompositionGateway` shows how to create and host a .Net Core API Gateway, or reverse proxy, that composes http requests to multiple API backends. To run this sample ensure that the following projects are set as startup projects:

* `Divergent.Sales.API.Host`
* `Divergent.Shipping.API.Host`
* `Divergent.CompositionGateway`

As client to test the funzionality a REST client such as [Postman](https://chrome.google.com/webstore/detail/postman/fhbjgbiflinjbdggehcddcbncdddomop?hl=en) can be used.
The `Divergent.SPA` sample is a TypeScript based Single Page Application that can be used as a `Divergent.CompositionGateway`.

### Divergent.Frontend - sample

Similar to the `ASP.Net MVC` the `Divergent.Frontend` sample is a .Net Core Mvc app that composes http requests to multiple backends directly in the Mvc Views.  To run this sample ensure that the following projects are set as startup projects:

* `Divergent.Sales.API.Host`
* `Divergent.Shipping.API.Host`
* `Divergent.Frontend`
