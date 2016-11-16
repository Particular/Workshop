/*
 * itemTemplate Directive v 1.0.0.0
 *  Dependencies:
 *    --
 */
(function () {

    var module = angular.module('itemTemplate', []);

    module.provider('itemTemplateConfig', function itemTemplateConfigProvider() {

        var _defaultSettings = {
            templatesFolder: '/templates/',
            dataTypeProperty: 'dataType',
            defaultLoadingTemplate: '<span class="item-template-loading">Loading...</span>',
            defaultLoadingErrorTemplate: '<span>Cannot find any valid template for: {{templateModel}}</span>'
        };

        this.setDefaultSettings = function (userDefaultSetting) {
            angular.extend(_defaultSettings, userDefaultSetting);
        };

        this.$get = ['$log', '$http', '$templateCache', function itemTemplateConfig($log, $http, $templateCache) {
            return {
                defaultSettings: _defaultSettings,
                defaultTemplatesSelector: function (model, settings) {

                    var dataTypePropertyValue = 'undefined';
                    if (model) {
                        dataTypePropertyValue = model[settings.dataTypeProperty];
                    } else if (settings.undefinedTemplateName) {
                        dataTypePropertyValue = settings.undefinedTemplateName
                    }

                    var templateFileName = dataTypePropertyValue;
                    $log.debug('itemTemplate directive templateSelector dataTypePropertyValue:', dataTypePropertyValue);

                    if (settings.templatesMap) {
                        $log.debug('itemTemplate directive settings have a templatesMap:', settings.templatesMap);

                        if (settings.templatesMap[dataTypePropertyValue]) {
                            $log.debug('itemTemplate directive templateSelector template found in templatesMap');
                            templateFileName = settings.templatesMap[dataTypePropertyValue];
                        } else if (settings.templatesMap['undefined-template']) {
                            $log.debug('itemTemplate directive settings defines an undefined-template property.');
                            templateFileName = settings.templatesMap['undefined-template'];
                        }
                    }

                    if (settings.templateNamePrefix) {
                        $log.debug('itemTemplate directive settings have a templateNamePrefix:', settings.templateNamePrefix);
                        templateFileName = templateNamePrefix + templateFileName;
                    }

                    if (settings.templateNameSuffix) {
                        $log.debug('itemTemplate directive settings have a templateNameSuffix:', settings.templateNameSuffix);
                        templateFileName = templateFileName + settings.templateNameSuffix;
                    }

                    var templateUrl = settings.templatesFolder + templateFileName + '.html';
                    $log.debug('itemTemplate directive templateSelector templateUrl:', templateUrl);

                    return templateUrl;
                },
                defaultTemplatesLoader: function (templateUrl) {
                    return $http.get(templateUrl, { cache: $templateCache });
                }
            };
        }];

    });

    var itemTemplateDirective = function ($parse, $compile, $log, config) {

        $log.debug('itemTemplate directive injecting function:', config);

        var templatesSelectorProvider = function ($linkAttributes) {
            var selector = null;

            if ($linkAttributes.templateSelector) {
                var parsed = $parse($linkAttributes.templateSelector);
                selector = parsed;
                $log.debug('itemTemplate directive templateSelector found as attribute: ', $linkAttributes.templateSelector, selector);
            } else {
                $log.debug('itemTemplate directive templateSelector not found, creating default.');

                selector = config.defaultTemplatesSelector;
            }

            return selector;
        };

        var templatesLoaderProvider = function ($linkAttributes) {
            var loader = null;

            if ($linkAttributes.templatesLoader) {
                var parsed = $parse($linkAttributes.templatesLoader);
                loader = parsed;
                $log.debug('itemTemplate directive templatesLoader found as attribute: ', $linkAttributes.templatesLoader, loader);
            } else {
                $log.debug('itemTemplate directive templatesLoader not found, creating default.');

                loader = config.defaultTemplatesLoader;
            }

            return loader;
        };

        var settingsProvider = function ($linkAttributes) {

            var settings = null;
            if ($linkAttributes.itemTemplateSettings) {
                var parsed = $parse($linkAttributes.itemTemplateSettings);
                settings = parsed();

                $log.debug('itemTemplate directive settings found as attribute: ', $linkAttributes.itemTemplateSettings, settings);

                if (!settings.templatesFolder) {
                    $log.debug('itemTemplate directive settings templatesFolder is missing, setting default.');
                    settings.templatesFolder = config.defaultSettings.templatesFolder;
                } else {
                    $log.debug('itemTemplate directive settings templatesFolder found:', settings.templatesFolder);
                    var firstChar = settings.templatesFolder.substring(0, 1);
                    if (firstChar === '^') {
                        $log.debug('itemTemplate directive settings templatesFolder starts with the rebasing char ^.');
                        var newTemplatesFolder = config.defaultSettings.templatesFolder + settings.templatesFolder.substring(1);
                        settings.templatesFolder = newTemplatesFolder;
                        $log.debug('itemTemplate directive settings rebased templatesFolder:', settings.templatesFolder);
                    }
                }

                if (!settings.dataTypeProperty) {
                    $log.debug('itemTemplate directive settings dataTypeProperty is missing, setting default.');
                    settings.dataTypeProperty = config.defaultSettings.dataTypeProperty;
                }

            } else {
                $log.debug('itemTemplate directive settings not found, creating defaults.');
                settings = angular.copy(config.defaultSettings);
            }

            $log.debug('itemTemplate directive settings:', settings);

            return settings;
        };

        return {
            restrict: 'EA',
            transclude: false,
            replace: false,
            scope: {
                templateModel: '=',
                templateContext: '='
            },
            compile: function compiler($linkAttributes) {

                var link = {
                    post: function (scope, $linkElement, $linkAttributes) {
                        $log.debug('itemTemplate directive post linker function.');
                        $log.debug('[scope, $linkElement, $linkAttributes]', scope, $linkElement, $linkAttributes);

                        var settings = settingsProvider($linkAttributes);
                        var selector = templatesSelectorProvider($linkAttributes);
                        var loader = templatesLoaderProvider($linkAttributes);

                        scope.$watch('templateModel', function (model) {
                        $log.debug('itemTemplate directive templateModel changed [model]:', model);

                                if ( ( model === null || model === undefined) && !settings.handleUndefinedModel ) {
                                    $log.debug('itemTemplate directive templateModel is null, template, if any, will be destroyed.');
                                    $linkElement.empty();
                                } else {

                                    var templateUrl = selector(model, settings);

                                    var composer = function (html) {
                                        var temp = $(html);

                                        $linkElement.empty();

                                        if (temp.length > 1) {
                                            var wrapper = $('<div></div>');
                                            temp.appendTo(wrapper);

                                            wrapper.appendTo($linkElement);
                                        } else {
                                            temp.appendTo($linkElement);
                                        }

                                        var firstChild = $linkElement.children(0);
                                        var compiledElement = $compile($linkElement.html())(scope);

                                        firstChild.replaceWith(compiledElement);
                                    };

                                    composer(config.defaultSettings.defaultLoadingTemplate);

                                    loader(templateUrl)
                                        .success(function (data, status, headers, cfg) {
                                            composer(data);
                                        })
                                        .error(function (data, status, headers, cfg) {
                                            $log.error('template loading error: ', data, status, headers);
                                            composer(config.defaultSettings.defaultLoadingErrorTemplate);
                                        });
                                }  
                        });
                    }
                };

                return link;
            }
        };
    };

    module.directive('itemTemplate', ['$parse', '$compile', '$log', 'itemTemplateConfig', itemTemplateDirective]);

}());