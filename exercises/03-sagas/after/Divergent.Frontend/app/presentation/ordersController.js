(function () {
    angular.module('app.controllers')
        .controller('ordersController',
        ['$log', 'endpoints.config', '$http',
            function ($log, config, $http) {

                var ctrl = this;

                ctrl.isLoading = null;
                ctrl.orders = null;

                ctrl.refreshOrders = function () {
                    ctrl.isLoading = $http.get(config.gatewayBaseUrl + '/orders/')
                        .then(function (response) {
                            ctrl.orders = response.data.orders;
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

                    return $http.post(config.salesApiUrl + '/orders/createOrder', payload)
                         .then(function (createOrderResponse) {
                             $log.debug('raw order created:', createOrderResponse.data);

                             //this should allow all services to chime in

                             return createOrderResponse.data;
                         });
                };

                ctrl.refreshOrders();
            }]);
}())