(function () {

    function CustomerViewModel(customerReadModel) {
        var readModel = customerReadModel;
        this.dataType = 'customer';

        Object.defineProperty(this, 'displayName', {
            get: function () {
                return readModel.name;
            }
        });

        Object.defineProperty(this, 'id', {
            get: function () {
                return readModel.id;
            }
        });
    };

    angular.module('app.services')
        .constant('customers.config', {
            apiUrl: 'http://localhost:20186/api'
        });

    angular.module('app.services')
        .run(['$log', 'messageBroker', '$http', 'customers.config', function ($log, messageBroker, $http, config) {

            messageBroker.subscribe('first-order/executed', function (sender, args) {

                var uri = config.apiUrl + '/customers/' + args.rawData.customerId;
                $http.get(uri)
                     .then(function (response) {

                         $log.debug('HTTP response', response.data);

                         args.viewModel.customer = new CustomerViewModel(response.data);

                         $log.debug('Orders composed w/ Customers', args.viewModel);

                     });
            });

        }]);
}())