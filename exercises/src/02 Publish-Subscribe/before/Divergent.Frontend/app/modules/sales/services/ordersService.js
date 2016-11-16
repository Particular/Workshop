/* global angular */
(function () {

    'use strict';

    angular.module('app.services')
        .factory('ordersService',
        ['$log', 'messageBroker', 'backendCompositionService', '$http', 'orders.config',
            function ($log, messageBroker, backendCompositionService, $http, config) {

                var getOrders = function (pageIndex, pageSize) {

                    var promise = backendCompositionService
                        .get('orders-list', { pageIndex: pageIndex, pageSize: pageSize })
                        .then(function (composedResult) {
                            $log.debug('orders-list -> composedResult:', composedResult);
                            return composedResult.orders.all;
                        });

                    return promise;
                };

                var createNewOrder = function (payload) {

                    return $http.post(config.apiUrl + '/orders/createOrder', payload)
                        .then(function (createOrderResponse) {
                            $log.debug('raw order created:', createOrderResponse.data);

                            var rawOrder = createOrderResponse.data;
                            var orderVm = new OrderViewModel(rawOrder);

                            var args = {
                                rawData: rawOrder,
                                viewModel: orderVm
                            };

                            messageBroker.broadcast('orders/newOrderCreated', this, args);

                            return orderVm;
                        });
                };

                return {
                    getOrders: getOrders,
                    createNewOrder: createNewOrder
                }
            }]);

}())