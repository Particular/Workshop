(function () {
    angular.module('app.controllers')
        .controller('ordersController',
        ['$log', 'ordersService', 'messageBroker',
            function ($log, ordersService, messageBroker) {

                var vm = this;

                vm.isBusy = null;
                vm.list = null;

                vm.isBusy = ordersService
                    .getOrders(0, 10)
                    .then(function (orders) {
                        $log.debug('orders-list -> orders:', orders);
                        vm.list = orders;
                    });

                vm.createNewOrder = function () {

                    var payload = {
                        customerId: '24453089-e36d-41ff-b119-82ae57482c74', //Particular
                        products: [{
                            productId: '5e449efa-3f48-45d2-82f0-22483d97516a' // The Force Awakens
                        }]
                    };

                    ordersService.createNewOrder(payload)
                        .then(function (orderVm) {
                            $log.debug('createNewOrder -> orderVm:', orderVm);

                            orderVm.isPending = true;
                            vm.list.push(orderVm);

                            $log.debug('orders-list -> orders:', vm.list);
                        });
                };

            }]);
}())