(function () {

    //function PriceViewModel(priceReadModel) {
    //    var readModel = priceReadModel;

    //    Object.defineProperty(this, 'dataType', {
    //        get: function () {
    //            return 'price';
    //        }
    //    });

    //    Object.defineProperty(this, 'value', {
    //        get: function () {
    //            return priceReadModel;
    //        }
    //    });

    //    Object.defineProperty(this, 'currency', {
    //        get: function () {
    //            return '$';
    //        }
    //    });
    //};

    angular.module('app.services')
        .constant('finance.config', {
            apiUrl: 'http://localhost:20187/api'
        });

    angular.module('app.services')
        .config(['backendCompositionServiceProvider',
            function (backendCompositionServiceProvider) {

                var requestId = 'orders-list';
                backendCompositionServiceProvider.registerViewModelSubscriberFactory(requestId,
                    ['$log', '$http', 'finance.config', function ($log, $http, config) {

                        var subscriber = function (viewModel) {
                            viewModel.subscribe('orders/loaded', function (evt, ctrlViewModel, args) {

                                //$log.debug('orders/loaded handled by finance module:', args);

                                //go grab order totals for each order
                                //compose the VM


                            });
                        };

                        return subscriber;
                    }]);

            }]);

    //angular.module('app.services')
    //    .run(['$log', 'messageBroker', '$http', 'finance.config', function ($log, messageBroker, $http, config) {

    //        messageBroker.subscribe('orders-list/executed', function (sender, args) {

    //            angular.forEach(args.rawData, function (order, index) {

    //                var uri = config.apiUrl + '/prices/total/' + order.productIds;

    //                $http.get(uri)
    //                     .then(function (response) {

    //                         $log.debug('Total price HTTP response', response.data);

    //                         var vm = new PriceViewModel(response.data);
    //                         args.viewModels[order.id].price = vm;

    //                         $log.debug('Orders composed w/ Prices', args.viewModels);
    //                     });

    //            });
    //        });

    //    }]);
}())