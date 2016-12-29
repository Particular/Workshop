(function () {

    function OrderViewModel(orderReadModel) {
        var readModel = orderReadModel;

        Object.defineProperty(this, 'dataType', {
            get: function () {
                return 'order';
            }
        });

        Object.defineProperty(this, 'id', {
            get: function () {
                return readModel.id;
            }
        });

        Object.defineProperty(this, 'itemsCount', {
            get: function () {
                return readModel.itemsCount;
            }
        });
    };

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

                var queryId = 'first-order';
                backendCompositionServiceProvider.registerQueryHandlerFactory(queryId,
                    ['$log', '$http', 'messageBroker', 'orders.config', function ($log, $http, messageBroker, config) {

                        var handler = {
                            get: function (args, composedResults) {

                                $log.debug('Ready to handle ', queryId, ' args: ', args);
                                var uri = config.apiUrl + '/orders/first';
                                return $http.get(uri)
                                    .then(function (response) {

                                        $log.debug('HTTP response', response.data);
                                                                                
                                        composedResults.order = new OrderViewModel(response.data);

                                        messageBroker.broadcast(queryId + '/executed', this, {
                                            rawData: response.data,
                                            viewModel: composedResults.order
                                        });

                                        $log.debug('Query ', queryId, 'handled: ', composedResults);

                                        return composedResults;
                                    });

                            }
                        }

                        return handler;

                    }]);

                console.debug('Orders modules configured.');
            }]);
}())