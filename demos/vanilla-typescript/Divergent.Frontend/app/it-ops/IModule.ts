import { IRegisterComponents } from "./IRegisterComponents";
import { IRequestsGateway } from "./IRequestsGateway";

export interface IModule {
    init(componentsRegistry: IRegisterComponents, requestsGateway: IRequestsGateway): void;
}