(function () {

    angular.module('app.services')
        .constant('orders.config', {
            apiUrl: 'http://localhost:20185/api'
        });

    angular.module('app.services')
        .config(['backendCompositionServiceProvider',
            function (backendCompositionServiceProvider) {

                var requestId = 'orders-list';
                backendCompositionServiceProvider.registerViewModelAppenderFactory(requestId,
                    ['$log', '$http', 'orders.config', '$q', function ($log, $http, config, $q) {

                        var appender = {
                            append: function (args, viewModel) {

                                var uri = config.apiUrl + '/orders?p=' + args.pageIndex + '&s=' + args.pageSize;
                                return $http.get(uri)
                                    .then(function (response) {

                                        var ordersViewModelDictionary = {
                                            keys:[],
                                            values: []
                                        };

                                        angular.forEach(response.data, function (item, index) {

                                            var vm = {
                                                orderId: item.id,
                                                orderNumber: item.id,
                                                orderItemsCount: item.itemsCount
                                            };

                                            ordersViewModelDictionary.keys.push(vm.orderId);
                                            ordersViewModelDictionary.values.push(vm);

                                            ordersViewModelDictionary[vm.orderId] = vm;

                                        });

                                        viewModel.orders = ordersViewModelDictionary;

                                        var ordersLoadedPromise = viewModel.raiseEvent('orders/loaded', { ordersViewModelDictionary: ordersViewModelDictionary });

                                        var orderIdsGroupedByCustomer = _.chain(response.data)
                                            .map(function (raw) {
                                                return {
                                                    customerId: raw.customerId,
                                                    orderId: raw.id,
                                                };
                                            })
                                            .groupBy('customerId')
                                            .value();

                                        var customersIdsLoadedPromise = viewModel.raiseEvent('customers/ids/loaded', {
                                            ordersViewModelDictionary: ordersViewModelDictionary,
                                            customersId: orderIdsGroupedByCustomer
                                        });

                                        return $q.all(ordersLoadedPromise, customersIdsLoadedPromise);
                                    });

                            }
                        }

                        return appender;

                    }]);
            }]);
}())