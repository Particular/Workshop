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
                    ['$log', '$http', 'messageBroker', 'orders.config', function ($log, $http, messageBroker, config) {

                        var appender = {
                            append: function (args, viewModel) {

                                var uri = config.apiUrl + '/orders?p=' + args.pageIndex + '&s=' + args.pageSize;
                                return $http.get(uri)
                                    .then(function (response) {

                                        var ordersViewModel = {
                                            keys:[],
                                            values: []
                                        };

                                        angular.forEach(response.data, function (item, index) {

                                            var vm = {
                                                orderId: item.id,
                                                orderNumber: item.id,
                                                orderItemsCount: item.itemsCount
                                            };

                                            ordersViewModel.keys.push(vm.orderId);
                                            ordersViewModel.values.push(vm);

                                            ordersViewModel[vm.orderId] = vm;

                                        });
                                        viewModel.orders = ordersViewModel;

                                        messageBroker.broadcast(requestId + '/executed', this, {
                                            rawData: response.data,
                                            viewModels: ordersViewModel
                                        });

                                        //return viewModel;
                                    });

                            }
                        }

                        return appender;

                    }]);
            }]);
}())