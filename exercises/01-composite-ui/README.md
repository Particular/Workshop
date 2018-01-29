# Exercise 1: UI Composition

**Important: Before attempting the exercise, please ensure you have followed [the instructions for preparing your machine](/README.md#preparing-your-machine-for-the-workshop) and that you have read [the instructions for running the exercise solutions](/README.md#running-the-exercise-solutions).**

Vertical slices are autonomous and span across all layers starting from the UI, through all other application layers, and even the database.

## Overview

In this exercise you'll see that, when adding new elements, we operate within a single slice. This approach reduces the risk of unintentionally breaking unrelated elements.

The application consists of three vertical slices:
- Customers
- Sales
- Finance

## Start-up projects

For more info, please see [the instructions for running the exercise solutions](/README.md#running-the-exercise-solutions).

* Divergent.CompositionGateway
* Divergent.Customers.API
* Divergent.Finance.API
* Divergent.Frontend
* Divergent.Sales.API

## Business requirements

The application UI consists of two pages: Dashboard and Orders. In this exercise you'll display additional information in the Orders page by modifying the API, view model and view templates.

First, you'll display an additional property in the existing view: the number of items contained in the order. Then you'll create a whole new vertical slice responsible for calculating and displaying the total price of the items in the order.

## Exercise 1.1: display the count of items in an order

In this exercise we'll display the count of items in an order by retrieving it from the Sales vertical. We'll do that by returning the item count from `OrdersController` and by modifying the `OrdersListViewModelAppender` class and the `ordersView` HTML template.

### Step 1

Add an `ItemsCount` property to the anonymous object returned by `OrdersController.Get()` (`Divergent.Sales.API\Controllers\OrdersController.cs`):

```c#
ItemsCount = o.Items.Count
```

### Step 2

Add an `OrderItemsCount` property to the `MapToViewModelDictionary` method in the `OrdersListViewModelAppender` in the `Divergent.Sales.ViewModelComposition` project:

```csharp
dynamic viewModel = new ExpandoObject();
viewModel.OrderId = order.Id;
viewModel.OrderNumber = order.Id;
viewModel.OrderItemsCount = order.ItemsCount;
```

### Step 3

Update the `ordersView` list template (`Divergent.Frontend\app\presentation\ordersView.html`) to display the new information

```html
<br />
<i>Items count:</i> {{order.orderItemsCount}}
```

## Exercise 1.2: display the total price of an order

In this exercise you'll add a new vertical slice. In order to do so, you'll need to add a new `OrdersLoadedSubscriber` class in the `Divergent.Finance.ViewModelComposition` project.

For simplicity, the Finance API backend is already in place and the exercise concentrates on composition and UI related tasks.

### Step 1

Create a new class file in `Divergent.Finance.ViewModelComposition` named `OrdersLoadedSubscriber.cs`.

### Step 2

Add the following code to the class

```csharp
using Divergent.Sales.ViewModelComposition.Events;
using ITOps.Json;
using ITOps.ViewModelComposition;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Net.Http;

namespace Divergent.Finance.ViewModelComposition
{
    public class OrdersLoadedSubscriber : ISubscribeToCompositionEvents
    {
        // Very simple matching for the purpose of the exercise.
        public bool Matches(RouteData routeData, string httpMethod) =>
            HttpMethods.IsGet(httpMethod)
                && string.Equals((string)routeData.Values["controller"], "orders", StringComparison.OrdinalIgnoreCase)
                && !routeData.Values.ContainsKey("id");

        public void Subscribe(IPublishCompositionEvents publisher)
        {
            publisher.Subscribe<OrdersLoaded>(async (pageViewModel, ordersLoaded, routeData, query) =>
            {
                var orderIds = string.Join(",", ordersLoaded.OrderViewModelDictionary.Keys);

                // Hardcoded to simplify the exercise. In a production app, a config object could be injected.
                var url = $"http://localhost:20187/api/prices/orders/total?orderIds={orderIds}";
                var response = await new HttpClient().GetAsync(url);

                dynamic[] prices = await response.Content.AsExpandoArrayAsync();

                foreach (dynamic price in prices)
                {
                    ordersLoaded.OrderViewModelDictionary[price.OrderId].OrderTotalPrice = price.Amount;
                }
            });
        }
    }
}
```

### Step 3

Update the `ordersView` list template (`app\branding\orders\ordersView.html`) to display the new information:

```html
<br />
<strong>Order total:</strong> {{order.orderTotalPrice}}
```

#### Note:

For simplicity, ViewModel composition components, such as `Divergent.Finance.ViewModelComposition`, `Divergent.Sales.ViewModelComposition`, `Divergent.Sales.ViewModelComposition.Events` and `Divergent.Customers.ViewModelComposition`, are directly referenced by the `Divergent.CompositionGateway`. This approach is used to simplify the build process by letting Visual Studio automatically determine the project build order and to copy build outputs to the `Divergent.CompositionGateway` binaries folder. In a production environment this is expected to be managed by the build and deployment pipeline.

## Conclusion

In this exercise we've seen how to combine data from various autonomous services into a single user interface.

If you'd like to discuss this further, please don't hesitate to drop us a line in our [community discussion forum](https://discuss.particular.net/).
