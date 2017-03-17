/* global angular */
(function () {

    'use strict';

    angular.module('app.services')
        .factory('ordersService',
        ['$log', 'backendCompositionService', '$http', 'orders.config',
            function ($log, backendCompositionService, $http, config) {

                var firstOrder = function () {

                    var promise = backendCompositionService
                        .get('first-order', {  })
                        .then(function (composedResult) {
                            $log.debug('first-order -> composedResult:', composedResult);
                            return composedResult.order;
                        });

                    return promise;
                };

                return {
                    firstOrder: firstOrder
                }
            }]);

}())