(function () {
    angular.module('app.controllers')
        .controller('ordersController',
        ['$log', 'backendCompositionService', 'orders.config', '$http',
            function ($log, backendCompositionService, config, $http) {

                var ctrl = this;

                ctrl.isLoading = null;
                ctrl.orders = null;

                ctrl.refreshOrders = function () {
                    ctrl.isLoading = backendCompositionService
                        .get('orders-list', { pageIndex: 0, pageSize: 10 })
                        .then(function (viewModel) {
                            ctrl.orders = viewModel.orders;
                        })
                        .catch(function (error) {
                            $log.error('Something went wrong: ', error);
                            ctrl.loadError = 'Something went wrong. Look at the console log in your browser';
                        });
                };

                ctrl.createNewOrder = function () {

                    var payload = {
                        customerId: 1, //Particular. Valid values: 1,2,3
                        products: [{
                            productId: 4 //The Empire Strikes Back. Valid values: 1 -> 7
                        }]
                    };

                    return $http.post(config.apiUrl + '/orders/createOrder', payload)
                        .then(function (createOrderResponse) {
                            $log.debug('raw order created:', createOrderResponse.data);

                            //this should allow all services to chime in

                            return createOrderResponse.data;
                        });
                };

                ctrl.refreshOrders();
            }]);
}())