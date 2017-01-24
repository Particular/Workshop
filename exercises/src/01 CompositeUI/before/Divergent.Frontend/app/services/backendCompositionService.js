(function () {
    angular.module('app.services')
        .provider('backendCompositionService', function backendCompositionServiceProvider() {

            var queryHandlerFactories = {};
            var queryHandlers = {};

            var viewModelVisitorFactories = {};
            var viewModelVisitors = {};

            this.registerQueryHandlerFactory = function (queryId, factory) {

                if (!queryHandlerFactories.hasOwnProperty(queryId)) {
                    queryHandlerFactories[queryId] = [];
                }

                queryHandlerFactories[queryId].push(factory);
            };

            this.registerViewModelVisitorFactory = function (queryId, factory) {

                if (!viewModelVisitorFactories.hasOwnProperty(queryId)) {
                    viewModelVisitorFactories[queryId] = [];
                }

                viewModelVisitorFactories[queryId].push(factory);
            };

            this.$get = ['$log', '$injector', '$q', 'messageBroker',
                function backendCompositionServiceFactory($log, $injector, $q, messageBroker) {

                    $log.debug('backendCompositionServiceFactory');

                    var svc = {};

                    svc.get = function (queryId, args) {

                        var handlers = queryHandlers[queryId];
                        if (!handlers) {
                            var handlerFactories = queryHandlerFactories[queryId];
                            if (!handlerFactories) {
                                throw 'Cannot find any valid queryHandler or factory for "' + queryId + '"';
                            }

                            handlers = [];
                            angular.forEach(handlerFactories, function (handlerFactory, index) {
                                var handler = $injector.invoke(handlerFactory);
                                handlers.push(handler);
                            });

                            queryHandlers[queryId] = handlers;
                        }

                        var visitors = viewModelVisitors[queryId];
                        if (!visitors) {
                            visitors = [];
                            var visitorFactories = viewModelVisitorFactories[queryId];
                            if (visitorFactories) {
                                angular.forEach(visitorFactories, function (visitorFactory, index) {
                                    var visitor = $injector.invoke(visitorFactory);
                                    visitors.push(visitor);
                                });

                                viewModelVisitors[queryId] = visitors;
                            }
                        }

                        var composedResult = {};
                        var promises = [];

                        angular.forEach(handlers, function (handler, index) {

                            var handlerPromise = handler.query(args, composedResult);
                            if (!handlerPromise) {
                                throw 'query must return a promise.';
                            }

                            promises.push(handlerPromise);

                            handlerPromise
                                .then(function (rawData) {
                                    $log.debug('calling visitors, rawData: ', rawData, ' composedResult: ', composedResult);
                                    angular.forEach(visitors, function (visitor, index) {
                                        var vp = visitor.visit(args, composedResult, rawData);
                                        promises.push(vp);
                                    });
                                });
                        });

                        return $q.all(promises)
                            .then(function (_) {
                                $log.debug(queryId, '-> completed -> ComposedResult: ', composedResult);
                                return composedResult;
                            });
                    };

                    return svc;

                }];

        });
}())