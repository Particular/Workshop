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

                ctrl.refreshOrders();
            }]);
}())