(function () {

    function OrderViewModel(orderReadModel) {
        var readModel = orderReadModel;

        Object.defineProperty(this, 'dataType', {
            get: function () {
                return 'order';
            }
        });

        Object.defineProperty(this, 'id', {
            get: function () {
                return readModel.id;
            }
        });

        Object.defineProperty(this, 'itemsCount', {
            get: function () {
                return readModel.itemsCount;
            }
        });
    };

    angular.module('app.services')
        .constant('orders.config', {
            apiUrl: 'http://localhost:20185/api'
        });

    angular.module('app.services')
        .config(['backendCompositionServiceProvider',
            function (backendCompositionServiceProvider) {

                var requestId = 'orders-list';
                backendCompositionServiceProvider.registerViewModelAppenderFactory(requestId,
                    ['$log', '$http', 'messageBroker', 'orders.config', function ($log, $http, messageBroker, config) {

                        var appender = {
                            append: function (args, viewModel) {

                                $log.debug('Ready to handle ', requestId, ' args: ', args);
                                var uri = config.apiUrl + '/orders?p=' + args.pageIndex + '&s=' + args.pageSize;
                                return $http.get(uri)
                                    .then(function (response) {

                                        $log.debug('HTTP response', response.data);

                                        var vms = {
                                            all: []
                                        };

                                        angular.forEach(response.data, function (item, index) {
                                            var vm = new OrderViewModel(item);
                                            vms.all.push(vm);
                                            vms[vm.id] = vm;

                                        });
                                        viewModel.orders = vms;

                                        messageBroker.broadcast(requestId + '/executed', this, {
                                            rawData: response.data,
                                            viewModels: vms
                                        });

                                        $log.debug('Query ', requestId, 'handled: ', viewModel);

                                        return viewModel;
                                    });

                            }
                        }

                        return appender;

                    }]);
            }]);
}())