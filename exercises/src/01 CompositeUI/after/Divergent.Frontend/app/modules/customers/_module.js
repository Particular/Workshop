(function () {

    angular.module('app.services')
        .constant('customers.config', {
            apiUrl: 'http://localhost:20186/api'
        });

    angular.module('app.services')
     .config(['backendCompositionServiceProvider',
         function (backendCompositionServiceProvider) {

             var requestId = 'orders-list';
             backendCompositionServiceProvider.registerViewModelSubscriberFactory(requestId,
                 ['$log', '$http', 'customers.config', function ($log, $http, config) {

                     var subscriber = function (viewModel) {
                         viewModel.subscribe('customers/ids/loaded', function (evt, ctrlViewModel, args) {
                             $log.debug('customers/ids/loaded handled by customers module:', args);

                             //go grab customer details for each order
                             var ids = '';
                             for (var key in args.customersId) {
                                 if (args.customersId.hasOwnProperty(key)) {
                                     ids += '|' + key;
                                 }
                             }
                             ids = ids.substring(1);

                             var uri = config.apiUrl + '/customers/byids/' + ids;
                             return $http.get(uri)
                                 .then(function (response) {
                                     
                                     angular.forEach(response.data, function (item, index) {

                                         var ordersId = args.customersId[item.id]

                                         angular.forEach(ordersId, function (order, key) {

                                             var orderViewModel = args.ordersViewModelDictionary[order.orderId];
                                             orderViewModel.customerName = item.name;
                                             orderViewModel.customerId = item.id;
                                         });
                                     });
                                 });

                         });
                     };

                     return subscriber;
                 }]);

         }]);
}())