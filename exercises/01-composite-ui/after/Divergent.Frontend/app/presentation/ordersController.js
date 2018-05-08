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
                        .then(function successCallback(response) {
                            ctrl.orders = response.data.orders;
                        }, function errorCallback(response) {
                            $log.error('Something went wrong: ', response);
                            var compositionErrors = response.headers('composition-errors');
                            if (compositionErrors)
                            {
                                $log.error('Composition errors: ', $('<textarea/>').html(compositionErrors).text());
                                ctrl.loadError = 'Something went wrong during the composition process. Look at the console log in your browser.';
                            }
                            else
                            {
                                ctrl.loadError = 'Something went wrong. Look at the console log in your browser';
                            }
                        });
                };

                ctrl.refreshOrders();
            }]);
}())