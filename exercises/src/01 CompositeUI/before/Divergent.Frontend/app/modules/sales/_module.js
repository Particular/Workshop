(function () {

    angular.module('app.services')
        .constant('orders.config', {
            apiUrl: 'http://localhost:20185/api'
        });

}())