# Coding Sessions - UI Decomposition

## Exercise 1 - add new information coming from an existing service

### Introduction

The vertical slices are autonomous and span across all layers - starting from the UI, through all other application layers, and even in the database. In this exercise you'll see that when adding new elements we operate within a single slice. That approach reduces the risk of unintentionally breaking unrelated elements.

The application consists of three vertical slices:
- Customers
- Sales
- Finance

### Business requirements

The application UI consists of two pages - Dashboard and Orders. In this exercise you'll display additional information in the the Orders page. In order to do so, you'll need to modify API, view model and view templates. 

First, you'll display an additional property in the existing view - the number of items contained in the order. Then you'll create a whole new vertical slice responsible for calculating and displaying the total price of items in the order.

### Exercise 01.1 - Displaying items count

In this exercise we'll display additional information retrieved from the Sales vertical. In order to do it, return Items count from `OrdersController`, modify `OrderViewModel` and `order.html` view.

NOTE: You could follow a different naming convention, but for simplicity the included files are hard-coded in the exercise. In a real-life project you'll probably use tools like `grunt` and `gulp` to automate the process. If you decide to use different names or location for new files you'll need to adjust the entry in the `index.html` file, which expects to find the following files:
```
<!-- Finance module -->
<script src="/app/modules/finance/_module.js" type="text/javascript"></script>
<script src="/app/modules/finance/viewModels.js" type="text/javascript"></script>
```

**1)** Add `ItemsCount` property to the anonymous object returned by `OrdersController` `Get` method. (`Divergent.Sales.API\Controllers\OrdersController.cs`)

		ItemsCount = o.Items.Count

**2)** Add an `itemsCount` read-only property to the js `OrderViewModel` (in `app\modules\sales\_module.js`):

		Object.defineProperty(this, 'itemsCount', {
	        get: function () {
	            return readModel.itemsCount;
	        }
	    });

**3)** Update the `order` template (`app\templates\sales\ordersView\order.html`) to display the new information

	    <strong>Items count:</strong> {{templateModel.itemsCount}}


## Exercise 01.2 - Displaying total price

In this exercise you'll add a new vertical slice. In order to do so, you'll need to add a new module called `finance` in the `Divergent.Frontend.SPA` project, add new `price.html` template and include it inside the existing `order.html` view.  Have a look at `customers` module in the `app\modules\sales` directory to see what files you're expected to provide and what should they contain. In order to retrieve total price for the given order, use the `PricingController` class in `Finance.API`.

**1)** Create the `app\modules\finance\_module.js` file in the `Divergent.Frontend.SPA` web site.

**2)** Add the following code to the module

		(function () {

		    function PriceViewModel(priceReadModel) {
		        var readModel = priceReadModel;

		        Object.defineProperty(this, 'dataType', {
		            get: function () {
		                return 'price';
		            }
		        });

		        Object.defineProperty(this, 'value', {
		            get: function () {
		                return priceReadModel;
		            }
		        });

		        Object.defineProperty(this, 'currency', {
		            get: function () {
		                return '$';
		            }
		        });
		    };

		    angular.module('app.services')
		        .constant('finance.config', {
		            apiUrl: 'http://localhost:20187/api'
		        });

		    angular.module('app.services')
		        .config(['$stateProvider', 'backendCompositionServiceProvider',
		            function ($stateProvider, backendCompositionServiceProvider) {
		                console.debug('Finance modules configured.');
		            }]);

		    angular.module('app.services')
		        .run(['$log', 'messageBroker', '$http', 'finance.config', function ($log, messageBroker, $http, config) {

		            messageBroker.subscribe('orders-list/executed', function (sender, args) {

		                angular.forEach(args.rawData, function (order, index) {

		                    var uri = config.apiUrl + '/prices/total/' + order.productIds;

		                    $http.get(uri)
		                         .then(function (response) {

		                             $log.debug('Total price HTTP response', response.data);

		                             var vm = new PriceViewModel(response.data);
		                             args.viewModels[order.id].price = vm;

		                             $log.debug('Orders composed w/ Prices', args.viewModels);
		                         });

		                });
		            });

		        }]);
			}())

The content of the module file is available in the solution folder as `finance_module.js.txt`

**3)** Create the `price.html` template file in  `app\templates\sales\ordersView`

**4)** Add the following html fragment to the newly created template:

	    <span>
		    <strong>Order total:</strong> {{templateModel.currency}} {{templateModel.value}}
	    </span>

The content of the template file is available in the solution folder as `finance_price.html.txt`

**5)** Edit the `order.html` template in the same folder (`app\templates\sales\ordersView`) to display the new template:

	    <item-template item-template-settings="{ templatesFolder: '^sales/ordersView/' }"
	                   template-model="templateModel.price"></item-template>

### Solution configuration

#### Start-up projects

* Divergent.Finance.API
* Divergent.Customers.API
* Divergent.Sales.API
* Divergent.Frontend.SPA
