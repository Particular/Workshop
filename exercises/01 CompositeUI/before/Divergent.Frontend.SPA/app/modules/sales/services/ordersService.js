/* global angular */
(function () {

    'use strict';

    angular.module('app.services')
        .factory('ordersService',
        ['$log', 'backendCompositionService', '$http', 'orders.config',
            function ($log, backendCompositionService, $http, config) {

                var getOrders = function (pageIndex, pageSize) {

                    var promise = backendCompositionService
                        .get('orders-list', { pageIndex: pageIndex, pageSize: pageSize })
                        .then(function (composedResult) {
                            $log.debug('orders-list -> composedResult:', composedResult);
                            return composedResult.orders.all;
                        });

                    return promise;
                };

                return {
                    getOrders: getOrders
                }
            }]);

}())