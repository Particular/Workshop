(function () {

    function PriceViewModel(priceReadModel) {
        var readModel = priceReadModel;

        Object.defineProperty(this, 'dataType', {
            get: function () {
                return 'price';
            }
        });

        Object.defineProperty(this, 'value', {
            get: function () {
                return priceReadModel;
            }
        });

        Object.defineProperty(this, 'currency', {
            get: function () {
                return '$';
            }
        });
    };

    angular.module('app.services')
        .constant('finance.config', {
            apiUrl: 'http://localhost:20187/api'
        });

    angular.module('app.services')
        .config(['$stateProvider', 'backendCompositionServiceProvider',
            function ($stateProvider, backendCompositionServiceProvider) {
                console.debug('Finance modules configured.');
            }]);

    angular.module('app.services')
        .run(['$log', 'messageBroker', '$http', 'finance.config', function ($log, messageBroker, $http, config) {

            messageBroker.subscribe('orders-list/executed', function (sender, args) {

                angular.forEach(args.rawData, function (order, index) {

                    var uri = config.apiUrl + '/prices/total/' + order.productIds;

                    $http.get(uri)
                         .then(function (response) {

                             $log.debug('Total price HTTP response', response.data);

                             var vm = new PriceViewModel(response.data);
                             args.viewModels[order.id].price = vm;

                             $log.debug('Orders composed w/ Prices', args.viewModels);
                         });

                });
            });

        }]);
}())