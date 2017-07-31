(function () {
    angular.module('app.controllers')
        .controller('ordersController',
        ['$log', 'backendCompositionService',
            function ($log, backendCompositionService) {

                var ctrl = this;

                ctrl.loadingData = true;
                ctrl.orders = null;

                backendCompositionService
                    .get('orders-list', { pageIndex: 0, pageSize: 10 })
                    .then(function (viewModel) {
                        ctrl.orders = viewModel.orders;
                    })
                    .catch(function (error) {
                        $log.error('Ooops, something went wroing: ', error);
                    })
                    .finally(function () {
                        ctrl.loadingData = false;
                    });

            }]);
}())