/* global angular */
(function () {

    angular.module('app.services')
        .directive('orderCustomer', ['$log',
            function ($log) {
                $log.debug('orderCustomer directive');

                return {
                    restrict: 'E',
                    scope: {
                        model: '=',
                    },
                    templateUrl: '/app/modules/customers/components/orderCustomer.html'
                };
            }]);

}())