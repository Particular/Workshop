import { IViewModelAppender } from "./IViewModelAppender";
import { ISubscribeToCompositionEvents } from "./ISubscribeToCompositionEvents";

export interface IRegisterComponents {
    registerAppender(appender: IViewModelAppender): void,
    registerSubscriber(subscriber: ISubscribeToCompositionEvents): void
}