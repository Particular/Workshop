(function () {
    angular.module('app.controllers')
        .controller('ordersController',
        ['$log', 'backendCompositionService',
            function ($log, backendCompositionService) {

                var ctrl = this;

                ctrl.isLoading = null;
                ctrl.orders = null;

                ctrl.isLoading = backendCompositionService
                    .get('orders-list', { pageIndex: 0, pageSize: 10 })
                    .then(function (viewModel) {
                        ctrl.orders = viewModel.orders;
                    })
                    .catch(function (error) {
                        $log.error('Something went wrong: ', error);
                        ctrl.loadError = 'Something went wrong. Look at the console log in your browser';
                    });

            }]);
}())