(function () {

    angular.module('app.services')
        .config(['backendCompositionServiceProvider',
            function (backendCompositionServiceProvider) {

                var requestId = 'orders-list';
                backendCompositionServiceProvider.registerViewModelSubscriberFactory(requestId,
                    ['$log', '$http', 'finance.config', function ($log, $http, config) {

                        var subscriber = function (viewModel) {
                            viewModel.subscribe('orders/loaded', function (evt, ctrlViewModel, args) {

                                var orderIds = args.ordersViewModelDictionary.keys;

                                var uri = config.apiUrl + '/prices/orders/total?orderIds=' + orderIds;
                                return $http.get(uri)
                                    .then(function (response) {

                                        angular.forEach(response.data, function (value, key) {
                                            args.ordersViewModelDictionary[key].orderTotalPrice = value;
                                        });
                                    });
                            });
                        };

                        return subscriber;
                    }]);

            }]);
}())