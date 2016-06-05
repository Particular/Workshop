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
                             var vm = new CustomerViewModel(item);
                             var orders = groupedByCustomerId[vm.id];

                             $log.debug('Orders for customer', vm.id, orders);

                             angular.forEach(orders, function (order, orderIdx) {
                                 args.viewModels[order.id].customer = vm;
                             });
                         });

                         $log.debug('Orders composed w/ Customers', args.viewModels);

                     });
            });

        }]);
}())