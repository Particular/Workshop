(function () {
    angular.module('app.controllers')
        .controller('ordersController',
        ['$log', 'ordersService',
            function ($log, ordersService) {

                var viewModel = this;

                viewModel.isBusy = null;
                viewModel.orders = null;

                viewModel.isBusy = ordersService
                    .getOrders(0, 10)
                    .then(function (orders) {
                        viewModel.orders = orders;
                    });

            }]);
}())