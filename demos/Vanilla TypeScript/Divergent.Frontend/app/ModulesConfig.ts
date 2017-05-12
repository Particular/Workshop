import { IModule } from "./it-ops/IModule";
import SalesModules from "./modules/sales/__module";

const modules: Array<IModule> = [
    new SalesModules()
];

export default class ModulesConfig {
    static GetModules(): Array<IModule> {
        return modules;
    }
};