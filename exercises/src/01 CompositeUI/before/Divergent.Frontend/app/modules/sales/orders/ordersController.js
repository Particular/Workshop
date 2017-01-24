(function () {
    angular.module('app.controllers')
        .controller('ordersController',
        ['$log', 'backendCompositionService',
            function ($log, backendCompositionService) {

                var ctrl = this;

                ctrl.isBusy = null;
                ctrl.orders = null;

                ctrl.isBusy = backendCompositionService
                    .get('orders-list', { pageIndex: 0, pageSize: 10 })
                    .then(function (viewModel) {
                        ctrl.orders = viewModel.orders;
                    });

            }]);
}())