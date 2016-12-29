(function () {
    angular.module('app.controllers')
        .controller('ordersController',
        ['$log', 'ordersService',
            function ($log, ordersService) {

                var vm = this;

                vm.isBusy = null;
                vm.order = null;

                vm.isBusy = ordersService
                    .firstOrder()
                    .then(function (order) {
                        $log.debug('orders-list -> orders:', order);
                        vm.order = order;
                    });

            }]);
}())