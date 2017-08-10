# Exercise 1: UI Composition

**Important: Before attempting the exercise, please ensure you have followed [the instructions for preparing your machine](README.md#preparing-your-machine-for-the-workshop) and that you have read [the instructions for running the exercise solutions](/README.md#running-the-exercise-solutions).**

Vertical slices are autonomous and span across all layers starting from the UI, through all other application layers, and even the database.

## Overview

In this exercise you'll see that, when adding new elements, we operate within a single slice. This approach reduces the risk of unintentionally breaking unrelated elements.

The application consists of three vertical slices:
- Customers
- Sales
- Finance

## Start-up projects

For more info, please see [the instructions for running the exercise solutions](/README.md#running-the-exercise-solutions).

* Divergent.Customers.API
* Divergent.Finance.API
* Divergent.Frontend
* Divergent.Sales.API

## Business requirements

The application UI consists of two pages: Dashboard and Orders. In this exercise you'll display additional information in the Orders page by modifying the API, view model and view templates. 

First, you'll display an additional property in the existing view: the number of items contained in the order. Then you'll create a whole new vertical slice responsible for calculating and displaying the total price of the items in the order.

## Exercise 1.1: display the count of items in an order

In this exercise we'll display the count of items in an order by retrieving it from the Sales vertical. We'll do that by returning the item count from `OrdersController` and by modifying `orderListAppender` and `ordersView`.

NOTE: You could follow a different naming convention but, for simplicity, the included files are hard-coded in the exercise. In a real-life project you'll probably use tools like `grunt` and `gulp` to automate the process. If you decide to use different names or locations for new files, you'll need to adjust the paths in `index.html`:

```html
<!-- Finance module -->
<script src="/app/modules/finance/_module.js" type="text/javascript"></script>
<script src="/app/modules/finance/ordersLoadedSubscriber.js" type="text/javascript"></script>
```

### Step 1

Add an `ItemsCount` property to the anonymous object returned by `OrdersController.Get()` (`Divergent.Sales.API\Controllers\OrdersController.cs`):

```c#
ItemsCount = o.Items.Count
```

### Step 2

Add an `orderItemsCount` property to the `mapToDictionary` function (in `Divergent.Frontend\app\modules\sales\orderListAppender.js`):

```js
var vm = {
    orderId: item.id,
    orderNumber: item.id,
    orderItemsCount: item.itemsCount
};
```

Note that the client side property is camelCased: "itemsCount", whereas the server side property is PascalCased: "ItemsCount".

### Step 3

Update the `orders` list template (`Divergent.Frontend\app\branding\orders\ordersView.html`) to display the new information

```html
<br />
<i>Items count:</i> {{order.orderItemsCount}}
```

## Exercise 1.2: display the total price of an order

In this exercise you'll add a new vertical slice. In order to do so, you'll need to add a new module called `finance` in the `Divergent.Frontend` project.

### Step 1

Create a new file named in `Divergent.Frontend\app\modules\finance` named  `ordersLoadedSubscriber.js`.

### Step 2

Add the following code to the module

```js
(function () {

    angular.module('app.services')
        .config(['backendCompositionServiceProvider',
            function (backendCompositionServiceProvider) {

                var requestId = 'orders-list';
                backendCompositionServiceProvider.registerViewModelSubscriberFactory(requestId,
                    ['$log', '$http', 'finance.config', function ($log, $http, config) {

                        var subscriber = function (viewModel) {
                            viewModel.subscribe('orders/loaded', function (evt, ctrlViewModel, args) {

                                var orderIds = args.ordersViewModelDictionary.keys;

                                var uri = config.apiUrl + '/prices/orders/total?orderIds=' + orderIds;
                                $http.get(uri)
                                    .then(function (response) {

                                        angular.forEach(response.data, function (value, key) {
                                            args.ordersViewModelDictionary[key].orderTotalPrice = value;
                                        });
                                    });
                            });
                        };

                        return subscriber;
                    }]);

            }]);
}())
```

### Step 3

Update the `orders` list template (`app\branding\orders\ordersView.html`) to display the new information:

```html
<br />
<strong>Order total:</strong> {{order.orderTotalPrice}}
```
