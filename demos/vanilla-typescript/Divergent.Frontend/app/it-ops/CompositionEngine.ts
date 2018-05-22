import { IViewModelAppender } from "./IViewModelAppender";
import { ISubscribeToCompositionEvents } from "./ISubscribeToCompositionEvents";
import { IRegisterComponents } from "./IRegisterComponents";
import { IRequestsGateway } from "./IRequestsGateway";

export default class CompositionEngine
    implements IRegisterComponents, IRequestsGateway {

    private appendersRegistry: { [id: string]: IViewModelAppender[]; } = {};
    private subscribersRegistry: { [id: string]: ISubscribeToCompositionEvents[]; } = {};

    registerAppender(appender: IViewModelAppender): void {

        let appenders = this.appendersRegistry[appender.requestIdentifier];
        if (appenders == null) {
            appenders = [];
            this.appendersRegistry[appender.requestIdentifier] = appenders;
        }

        appenders.push(appender);

        window.console.debug("Registered appender:", this.appendersRegistry);
    }

    registerSubscriber(subscriber: ISubscribeToCompositionEvents): void {

        let subscribers = this.subscribersRegistry[subscriber.requestIdentifier];
        if (subscribers == null) {
            subscribers = [];
            this.subscribersRegistry[subscriber.requestIdentifier] = subscribers;
        }

        subscribers.push(subscriber);

        window.console.debug("Registered subscriber:", this.subscribersRegistry);
    }
}