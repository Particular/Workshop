(function () {

    angular.module('app.services')
        .config(['backendCompositionServiceProvider',
            function (backendCompositionServiceProvider) {

                var requestId = 'orders-list';
                backendCompositionServiceProvider.registerViewModelSubscriberFactory(requestId,
                    ['$log', '$http', 'customers.config', function ($log, $http, config) {

                        var subscriber = function (viewModel) {
                            viewModel.subscribe('orders/loaded', function (evt, ctrlViewModel, args) {

                                var orderIds = args.ordersViewModelDictionary.keys;

                                var uri = config.apiUrl + '/customers/byorders?orderIds=' + orderIds;
                                $http.get(uri)
                                    .then(function (response) {

                                        angular.forEach(response.data, function (value, key) {
                                            args.ordersViewModelDictionary[key].orderCustomerName = value.name;
                                        });
                                    });
                            });
                        };

                        return subscriber;
                    }]);

            }]);
}())