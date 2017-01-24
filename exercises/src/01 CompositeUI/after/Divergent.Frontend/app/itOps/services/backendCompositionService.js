(function () {
    angular.module('app.services')
        .provider('backendCompositionService', function backendCompositionServiceProvider() {

            var viewModelAppendersFactories = {};
            var viewModelAppenders = {};

            this.registerViewModelAppenderFactory = function (requestId, appenderFactory) {

                if (!viewModelAppendersFactories.hasOwnProperty(requestId)) {
                    viewModelAppendersFactories[requestId] = [];
                }

                viewModelAppendersFactories[requestId].push(appenderFactory);
            };

            var viewModelSubscribersFactories = {};
            var viewModelSubscribers = {};

            this.registerViewModelSubscriberFactory = function (requestId, subscriberFactory) {

                if (!viewModelSubscribersFactories.hasOwnProperty(requestId)) {
                    viewModelSubscribersFactories[requestId] = [];
                }

                viewModelSubscribersFactories[requestId].push(subscriberFactory);
            };

            this.$get = ['$log', '$injector', '$q',
                function backendCompositionServiceFactory($log, $injector, $q) {

                    $log.debug('backendCompositionServiceFactory');

                    var svc = {};

                    svc.get = function (requestId, args) {

                        var appenders = viewModelAppenders[requestId];
                        if (!appenders) {
                            var factories = viewModelAppendersFactories[requestId];
                            if (!factories) {
                                throw 'Cannot find any valid viewModelAppender or factory for "' + requestId + '"';
                            }

                            appenders = [];
                            angular.forEach(factories, function (factory, index) {
                                var appender = $injector.invoke(factory);
                                appenders.push(appender);
                            });

                            viewModelAppenders[requestId] = appenders;
                        }

                        var subscribers = viewModelSubscribers[requestId];
                        if (!subscribers) {
                            var factories = viewModelSubscribersFactories[requestId];
                            if (!factories) {
                                $log.debug('Cannot find any valid viewModelSubscriber or factory for "' + requestId + '"');
                            }
                            else {
                                subscribers = [];
                                angular.forEach(factories, function (factory, index) {
                                    var subscriber = $injector.invoke(factory);
                                    subscribers.push(subscriber);
                                });

                                viewModelSubscribers[requestId] = subscribers;
                            }
                        }

                        var viewModel = {
                            subscribe: function () { },
                            raiseEvent: function () { },
                        };

                        var promises = [];

                        angular.forEach(appenders, function (appender, index) {

                            var promise = appender.append(args, viewModel);
                            promises.push(promise);
                        });

                        return $q.all(promises)
                            .then(function (_) {
                                $log.debug(requestId, '-> completed -> viewModel: ', viewModel);
                                return viewModel;
                            });
                    };

                    return svc;

                }];

        });
}())