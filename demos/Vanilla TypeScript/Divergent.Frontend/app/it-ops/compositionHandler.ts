import { IViewModelAppender } from './IViewModelAppender';
import { ISubscribeToCompositionEvents } from './ISubscribeToCompositionEvents';

export default class CompositionHandler {

    private static appenders: { [id: string]: IViewModelAppender[]; } = {};
    private static subscribers: { [id: string]: ISubscribeToCompositionEvents[]; } = {};

    static registerAppender(type: string, appender: IViewModelAppender) {

        let appenders = CompositionHandler.appenders[type];
        if (appenders == null) {
            appenders = [];
            CompositionHandler.appenders[type] = appenders;
        }

        appenders.push(appender);

        console.debug('Registered appender:', CompositionHandler.appenders);
    }

    static registerSubscriber(type: string, subscriber: ISubscribeToCompositionEvents) {

        let subscribers = CompositionHandler.subscribers[type];
        if (subscribers == null) {
            subscribers = [];
            CompositionHandler.subscribers[type] = subscribers;
        }

        subscribers.push(subscriber);

        console.debug('Registered subscriber:', CompositionHandler.subscribers);
    }
}