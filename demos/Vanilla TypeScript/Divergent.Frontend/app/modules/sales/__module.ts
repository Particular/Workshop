import FooAppender from './FooAppender';
import CompositionHandler from '../../it-ops/compositionHandler';
import { IModule } from '../../it-ops/IModule';

export default class SalesModule implements IModule {
    init() {
        CompositionHandler.registerAppender('foo', new FooAppender());
        console.debug('Sales module...');
    }
}
