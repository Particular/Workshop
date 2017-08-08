/* global angular */
(function () {

    'use strict';

    angular.module('app.controllers', []);
    angular.module('app.services', []);

    var app = angular.module('app', [
        'ngRoute',
        'ui.router',
        'cgBusy',
        'app.controllers',
        'app.services'
    ]);

    app.run(['$log', '$rootScope',
        function ($log, $rootScope) {
            $rootScope.$log = $log;
            console.debug('app run.');
        }]);

}())