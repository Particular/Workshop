(function () {

    angular.module('app.services')
        .config(['backendCompositionServiceProvider',
            function (backendCompositionServiceProvider) {

                var requestId = 'orders-list';
                backendCompositionServiceProvider.registerViewModelAppenderFactory(requestId,
                    ['$log', '$http', 'orders.config', '$q', function ($log, $http, config, $q) {

                        var appender = {
                            append: function (args, viewModel) {

                                var uri = config.apiUrl + '/orders';
                                return $http.get(uri)
                                    .then(function (response) {

                                        viewModel.orders = mapToDictionary(response.data);

                                        var promise = viewModel.raiseEvent('orders/loaded', {
                                            ordersViewModelDictionary: viewModel.orders
                                        });

                                        return promise;

                                    });

                            }
                        }

                        function mapToDictionary(rawData) {

                            var ordersViewModelDictionary = {
                                keys: [],
                                values: []
                            };

                            angular.forEach(rawData, function (item, index) {

                                var vm = {
                                    orderId: item.id,
                                    orderNumber: item.id,
                                    orderItemsCount: item.itemsCount
                                };

                                ordersViewModelDictionary.keys.push(vm.orderId);
                                ordersViewModelDictionary.values.push(vm);

                                ordersViewModelDictionary[vm.orderId] = vm;

                            });

                            return ordersViewModelDictionary;
                        };

                        return appender;

                    }]);
            }]);
}())