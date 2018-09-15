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

The application UI consists of two pages: Dashboard and Orders. In this exercise, your goal is to enhance the Orders page by displaying the count of items in an order and the total price of an order.

## Exercise 1.1: display the count of items in an order

`Divergent.Sales.Data\Models\Order.cs` already contains an `Items` collection. Your goal is to display the count of those items in the UI.

The solution involves modifying the API, the view model composition component, and the view template along the Sales vertical slice.

### Step 1 

In `Divergent.Sales.API\Controllers\OrdersController.cs`, add an `ItemsCount` property to the anonymous objects returned by `Get()`:

```c#
.Select(order => new
{
    ...
    ItemsCount = order.Items.Count,
})
```

### Step 2

In `Divergent.Sales.ViewModelComposition/OrdersListViewModelAppender.cs`, add an `OrderItemsCount` property to the dynamic values returned by `MapToViewModelDictionary()`:

```csharp
dynamic viewModel = new ExpandoObject();
...
viewModel.OrderItemsCount = order.ItemsCount;
```

### Step 3

Update the `Divergent.Frontend\wwwroot\app\presentation\ordersView.html` list template to display the count of items after the customer name.

```html
<br />
<i>Items count:</i> {{order.orderItemsCount}}
```

### Step 4

Run the solution. The count of items for each order should be displayed. If not, clear the browser cache and reload the page.

## Exercise 1.2: display the total price of an order

The Finance service is responsible for the order item prices. Your goal is to use those prices to calculate the total price for each order and display it in the UI.

The solution involves modifying the view model composition components and the view template along the Finance vertical slice.

For simplicity, the Finance API backend is already in place and the exercise concentrates on composition and UI related tasks.

### Step 1 

After the orders are loaded by Sales, Finance must provide the prices of the items in those orders.

In the `Divergent.Finance.ViewModelComposition` project, add a new class named `OrdersLoadedSubscriber.cs`.

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

Update the `Divergent.Frontend\wwwroot\app\presentation\ordersView.html` list template to display the total price after the count of items.

```html
<br />
<strong>Order total:</strong> {{order.orderTotalPrice}}
```

#### Note:

For simplicity, ViewModel composition components, such as `Divergent.Finance.ViewModelComposition`, `Divergent.Sales.ViewModelComposition`, `Divergent.Sales.ViewModelComposition.Events` and `Divergent.Customers.ViewModelComposition`, are directly referenced by the `Divergent.CompositionGateway`. This approach is used to simplify the build process by letting Visual Studio automatically determine the project build order and to copy build outputs to the `Divergent.CompositionGateway` binaries folder. In a production environment this is expected to be managed by the build and deployment pipeline.

### Step 4

Run the solution. The total price of each order should be displayed. If not, clear the browser cache and reload the page.
 
## Conclusion

In this exercise we've seen how to combine data from various autonomous services into a single user interface.

If you'd like to discuss this further, please don't hesitate to drop us a line in our [community discussion forum](https://discuss.particular.net/).
