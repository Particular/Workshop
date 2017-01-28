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

                            _subscriptions: {},

                            subscribe: function (evt, handler) {
                                var _vm = this;
                                if (!_vm._subscriptions.hasOwnProperty(evt)) {
                                    _vm._subscriptions[evt] = [];
                                }

                                _vm._subscriptions[evt].push(handler);
                            },

                            clearSubscribers: function () {
                                var _vm = this;
                                _vm._subscriptions = {};
                            },

                            raiseEvent: function (evt, args) {

                                var _vm = this;
                                if (!_vm._subscriptions.hasOwnProperty(evt)) {
                                    //no subscribers
                                    return null;
                                }

                                var promises = [];
                                var subscribers = _vm._subscriptions[evt];
                                angular.forEach(subscribers, function (subscriber, index) {
                                    var promise = subscriber(evt, _vm, args);
                                    promises.push(promise);
                                });

                                return $q.all(promises);
                            },
                        };

                        angular.forEach(subscribers, function (subscriber, index) {
                            subscriber(viewModel);
                        });

                        var promises = [];

                        angular.forEach(appenders, function (appender, index) {
                            var promise = appender.append(args, viewModel);
                            promises.push(promise);
                        });

                        return $q.all(promises)
                            .then(function (_) {
                                viewModel.clearSubscribers();
                                return viewModel;
                            });
                    };

                    return svc;

                }];

        });
}())