(function () {

    angular.module('app.services')
        .constant('orders.config', {
            apiUrl: 'http://localhost:20185/api'
        });

    angular.module('app.services')
        .config(['$stateProvider', 'backendCompositionServiceProvider',
            function ($stateProvider, backendCompositionServiceProvider) {

                $stateProvider
                    .state('orders', {
                        url: '/orders',
                        views: {
                            '': {
                                templateUrl: '/app/modules/sales/presentation/ordersView.html',
                                controller: 'ordersController',
                                controllerAs: 'orders'
                            }
                        }
                    });

                var ordersListQueryId = 'orders-list';
                backendCompositionServiceProvider.registerQueryHandlerFactory(ordersListQueryId,
                    ['$log', '$http', 'messageBroker', 'orders.config', function ($log, $http, messageBroker, config) {

                        var handler = {
                            get: function (args, composedResults) {

                                $log.debug('Ready to handle ', ordersListQueryId, ' args: ', args);
                                var uri = config.apiUrl + '/orders?p=' + args.pageIndex + '&s=' + args.pageSize;
                                return $http.get(uri)
                                    .then(function (response) {

                                        $log.debug('HTTP response', response.data);

                                        var vms = {
                                            all: []
                                        };

                                        angular.forEach(response.data, function (item, index) {
                                            var vm = new OrderViewModel(angular.copy(item));
                                            vms.all.push(vm);
                                            vms[vm.id] = vm;

                                        });
                                        composedResults.orders = vms;

                                        messageBroker.broadcast(ordersListQueryId + '/executed', this, {
                                            rawData: response.data,
                                            viewModels: vms
                                        });

                                        $log.debug('Query ', ordersListQueryId, 'handled: ', composedResults);

                                        return composedResults;
                                    });

                            }
                        }

                        return handler;

                    }]);

                console.debug('Orders modules configured.');
            }]);
}())