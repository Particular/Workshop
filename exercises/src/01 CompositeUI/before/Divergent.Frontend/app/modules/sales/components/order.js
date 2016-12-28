/* global angular */
(function () {

    angular.module('app.services')
        .directive('order', ['$log',
            function ($log) {
                $log.debug('order directive');

                return {
                    restrict: 'E',
                    scope: {
                        model: '=',
                    },
                    templateUrl: '/app/modules/sales/components/order.html'
                };
            }]);

}())