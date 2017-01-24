(function () {

    angular.module('app.services')
        .constant('finance.config', {
            apiUrl: 'http://localhost:20187/api'
        });
}())