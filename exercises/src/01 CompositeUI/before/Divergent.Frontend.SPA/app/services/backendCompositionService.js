(function () {
    angular.module('app.services')
        .provider('backendCompositionService', function backendCompositionServiceProvider() {

            var queryHandlerFactories = {};
            var queryHandlers = {};

            this.registerQueryHandlerFactory = function (queryId, handlerFactory) {

                if (!queryHandlerFactories.hasOwnProperty(queryId)) {
                    queryHandlerFactories[queryId] = [];
                }

                queryHandlerFactories[queryId].push(handlerFactory);
            };

            this.$get = ['$log', '$injector', '$q',
                function backendCompositionServiceFactory($log, $injector, $q) {

                    $log.debug('backendCompositionServiceFactory');

                    var svc = {};

                    svc.get = function (queryId, args) {

                        var handlers = queryHandlers[queryId];
                        if (!handlers) {
                            var factories = queryHandlerFactories[queryId];
                            if (!factories) {
                                throw 'Cannot find any valid queryHandler or factory for "' + queryId + '"';
                            }

                            handlers = [];
                            angular.forEach(factories, function (factory, index) {
                                var handler = $injector.invoke(factory);
                                handlers.push(handler);
                            });

                            queryHandlers[queryId] = handlers;
                        }

                        var deferred = $q.defer();

                        var composedResult = {
                            dataType: 'root'
                        };
                        var promises = [];

                        angular.forEach(handlers, function (handler, index) {

                            var promise = handler.get(args, composedResult);
                            if (!promise) {
                                throw 'executeQuery must return a promise.';
                            }
                            promises.push(promise);
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