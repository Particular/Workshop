(function () {

    angular.module('app.services')
        .constant('customers.config', {
            apiUrl: 'http://localhost:20186/api'
        });

    angular.module('app.services')
        .config(['$stateProvider', 'backendCompositionServiceProvider',
            function ($stateProvider, backendCompositionServiceProvider) {

                console.debug('Orders modules configured.');
            }]);

    angular.module('app.services')
        .run(['$log', 'messageBroker', '$http', 'customers.config', function ($log, messageBroker, $http, config) {

            messageBroker.subscribe('orders-list/executed', function (sender, args) {

                var groupedByCustomerId = _.groupBy(args.rawData, 'customerId');

                var customerUniqueIds = _.chain(args.rawData)
                    .map(function (rawOrder) { return rawOrder.customerId; })
                    .uniq()
                    .reduce(function (memo, id) { return memo + '|' + id; }, '')
                    .value()
                    .substring(1);

                var uri = config.apiUrl + '/customers/byids/' + customerUniqueIds;
                $http.get(uri)
                     .then(function (response) {

                         $log.debug('HTTP response', response.data);

                         angular.forEach(response.data, function (item, index) {
                             var vm = new CustomerViewModel(angular.copy(item));
                             var orders = groupedByCustomerId[vm.id];

                             $log.debug('Orders for customer', vm.id, orders);

                             angular.forEach(orders, function (order, orderIdx) {
                                 args.viewModels[order.id].customer = vm;
                             });
                         });

                         $log.debug('Orders composed w/ Customers', args.viewModels);

                     });
            });

            messageBroker.subscribe('orders/newOrderCreated', function (sender, args) {

                var uri = config.apiUrl + '/customers/byids/' + args.rawData.customerId;

                $http.get(uri)
                     .then(function (response) {

                         $log.debug('HTTP response', response.data);

                         var firstAndOnly = response.data[0];
                         var vm = new CustomerViewModel(angular.copy(firstAndOnly));
                         args.viewModel.customer = vm

                         $log.debug('New orders composed w/ customer', args.viewModel);
                     });

            });
        }]);
}())