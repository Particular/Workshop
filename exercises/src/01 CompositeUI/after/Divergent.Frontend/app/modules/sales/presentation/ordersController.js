(function () {
    angular.module('app.controllers')
        .controller('ordersController',
        ['$log', 'ordersService',
            function ($log, ordersService) {

                var vm = this;

                vm.isBusy = null;
                vm.list = null;

                vm.isBusy = ordersService
                    .getOrders(0, 10)
                    .then(function (orders) {
                        $log.debug('orders-list -> orders:', orders);
                        vm.list = orders;
                    });

            }]);
}())