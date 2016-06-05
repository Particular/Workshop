(function () {

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

                             var vm = new PriceViewModel(angular.copy(response.data));
                             args.viewModels[order.id].price = vm;

                             $log.debug('Orders composed w/ Prices', args.viewModels);
                         });

                });
            });

            messageBroker.subscribe('orders/newOrderCreated', function (sender, args) {

                var uri = config.apiUrl + '/prices/total/' + args.rawData.productIds;

                $http.get(uri)
                     .then(function (response) {

                         $log.debug('Total price HTTP response', response.data);

                         args.viewModel.price = new PriceViewModel(angular.copy(response.data));

                         $log.debug('new Order composed w/ price', args.viewModel);
                     });

            });

        }]);
}())