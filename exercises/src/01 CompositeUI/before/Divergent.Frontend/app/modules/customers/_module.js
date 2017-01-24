(function () {

    function CustomerViewModel(customerReadModel) {
        var readModel = customerReadModel;
        this.dataType = 'customer';

        Object.defineProperty(this, 'displayName', {
            get: function () {
                return readModel.name;
            }
        });

        Object.defineProperty(this, 'id', {
            get: function () {
                return readModel.id;
            }
        });
    };

    angular.module('app.services')
        .constant('customers.config', {
            apiUrl: 'http://localhost:20186/api'
        });

    angular.module('app.services')
        .config(['backendCompositionServiceProvider', function (backendCompositionServiceProvider) {

            var queryId = 'orders-list';
            backendCompositionServiceProvider.registerViewModelVisitorFactory(queryId,
                ['$log', '$http', 'customers.config', function ($log, $http, config) {

                    $log.debug('Registering Customers orders-list visitor');

                    var visitor = {
                        visit: function (args, composedResults, rawData) {

                            $log.debug('Customers - Ready to visit ', queryId, ': ', args, composedResults, rawData);

                            var groupedByCustomerId = _.groupBy(rawData, 'customerId');

                            var customerUniqueIds = _.chain(rawData)
                                .map(function (rawOrder) { return rawOrder.customerId; })
                                .uniq()
                                .reduce(function (memo, id) { return memo + '|' + id; }, '')
                                .value()
                                .substring(1);

                            var uri = config.apiUrl + '/customers/byids/' + customerUniqueIds;
                            $http.get(uri)
                                 .then(function (response) {

                                     $log.debug('Customers orders-list visitor HTTP response', response.data);

                                     angular.forEach(response.data, function (item, index) {
                                         var vm = new CustomerViewModel(item);
                                         var orders = groupedByCustomerId[vm.id];

                                         $log.debug('Orders for customer', vm.id, orders);

                                         angular.forEach(orders, function (order, orderIdx) {
                                             composedResults.orders[order.id].customer = vm;
                                         });
                                     });

                                     $log.debug('Orders composed w/ Customers', composedResults.orders);

                                     return response.data;
                                 });
                        }
                    }

                    return visitor;
                }]);

        }]);
}())