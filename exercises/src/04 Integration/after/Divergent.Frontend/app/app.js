/* global angular */
(function () {

    'use strict';

    angular.module('app.controllers', []);
    angular.module('app.services', []);

    var app = angular.module('app', [
                'ngRoute',
                'ui.router',
                'itemTemplate',
                'app.controllers',
                'app.services'
    ]);
    
    app.config(['$stateProvider', '$locationProvider', '$logProvider', 'itemTemplateConfigProvider',
            function ($stateProvider, $locationProvider, $logProvider, itemTemplateConfigProvider) {

                $logProvider.debugEnabled(true);
                $locationProvider.html5Mode(false);
                itemTemplateConfigProvider.setDefaultSettings({
                    templatesFolder: '/app/templates/'
                });

                var rootViews = {
                    '': {
                        templateUrl: '/app/presentation/dashboardView.html',
                        controller: 'dashboardController',
                        controllerAs: 'dashboard'
                    }
                };

                $stateProvider
                    .state('root', {
                        url: '',
                        views: rootViews
                    })
                    .state('dashboard', {
                        url: '/',
                        views: rootViews
                    });
                
                console.debug('app config.');
            }]);

    app.run(['$log', '$rootScope',
        function ($log, $rootScope) {
            $rootScope.$log = $log;
            
            console.debug('app run.');
        }]);

}())