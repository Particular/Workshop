import OrderDetailsAppender from "./OrderDetailsAppender";
import { IModule } from "../../it-ops/IModule";
import { IRegisterComponents } from "../../it-ops/IRegisterComponents";
import { IRequestsGateway } from "../../it-ops/IRequestsGateway";

export default class SalesModule implements IModule {

    init(componentsRegistry: IRegisterComponents, requestsGateway: IRequestsGateway) {

        componentsRegistry.registerAppender(new OrderDetailsAppender());

        window.console.debug("Sales module...");

    }

}
