# Coding Sessions - UI Decomposition

## Exercise 1 - add new information coming from an existing service

### Introduction

The vertical slices are autonomous and span across all layers - starting from the UI, through all other application layers, and even in the database. In this exercise you'll see that when adding new elements we operate within a single slice. That approach reduces the risk of unintentionally breaking unrelated elements.

The application consists of three vertical slices:
- Customers
- Sales
- Finance

### Business requirements

The application UI consists of two pages - Dashboard and Orders. In this exercise you'll display additional information in the Orders page. In order to do so, you'll need to modify the API, view model and view templates.

First, you'll display an additional property in the existing view - the number of items contained in the order. Then you'll create a whole new vertical slice responsible for calculating and displaying the total price of items in the order.

### Exercise 01.1 - Displaying items count

In this exercise we'll display additional information retrieved from the Sales vertical. In order to do it, return Items count from `OrdersController`, modify `orderListAppender` and `ordersView.html` view.

NOTE: You could follow a different naming convention, but for simplicity the included files are hard-coded in the exercise. In a real-life project you'll probably use tools like `grunt` and `gulp` to automate the process. If you decide to use different names or locations for new files you'll need to adjust the entry in the `index.html` file, which expects to find the following files:
```
<!-- Finance module -->
<script src="/app/modules/finance/_module.js" type="text/javascript"></script>
<script src="/app/modules/finance/ordersLoadedSubscriber.js" type="text/javascript"></script>
```

**1)** Add `ItemsCount` property to the anonymous object returned by `OrdersController` `Get` method. (`Divergent.Sales.API\Controllers\OrdersController.cs`)

		ItemsCount = o.Items.Count

**2)** Add a `orderItemsCount` property to the js `mapToDictionary` function (in `app\modules\sales\orderListAppender.js`):

	var vm = {
		orderId: item.id,
		orderNumber: item.id,
		orderItemsCount: item.itemsCount
	};

**3)** Update the `orders` list template (`app\branding\orders\ordersView.html`) to display the new information

	<br /><i>Items count:</i> {{order.orderItemsCount}}


## Exercise 01.2 - Displaying total price

In this exercise you'll add a new vertical slice. In order to do so, you'll need to add a new module called `finance` in the `Divergent.Frontend` project.

**1)** Create the `app\modules\finance\ordersLoadedSubscriber.js` file in the `Divergent.Frontend` web site.

**2)** Add the following code to the module

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

The content of the module file is available in the solution folder as `finance_ordersLoadedSubscriber.js.txt`

**3)** Update the `orders` list template (`app\branding\orders\ordersView.html`) to display the new information:

	<br /><strong>Order total:</strong> {{order.orderTotalPrice}}

The content of the template file is available in the solution folder as `finance_price.html.txt`

## Solution configuration

#### Start-up projects

* Divergent.Finance.API
* Divergent.Customers.API
* Divergent.Sales.API
* Divergent.Frontend
