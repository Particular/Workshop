/* global angular */
(function () {

    'use strict';
    
    function Subscription($log, eventType, unsubscribeHandler) {
        this.eventType = eventType
        this.unsubscribe = function () {
            
            var _self = this;
            
            $log.debug('unsubscribe from: ', _self.eventType);
            
            unsubscribeHandler();
        }
        
        $log.debug('subscription for ', eventType, ' created');
    }

    angular.module('app.services')
        .factory('messageBroker', ['$log', '$rootScope', function($log, $rootScope){
            
            var subscribe = function(eventType, eventHandler){
                
                $log.debug('subscription request for: ', eventType);
                
                var off = $rootScope.$on(eventType, function(evt, args){
                    
                    $log.debug('handling event ', eventType, args);
                    
                    eventHandler(args.sender, args.args);
                });
               
                return new Subscription($log, eventType, off);
            };
            var broadcast = function(eventType, sender, args){
                
                $log.debug('broadcasting event ', eventType, ' from ', sender,' with args: ', args);
                
                $rootScope.$broadcast(eventType, { 
                    sender: sender,
                    args: args 
                });
            };
            
            return {
                subscribe: subscribe,
                broadcast: broadcast
            }
        }]);

}())