import ModulesConfig from "./ModulesConfig";
import * as $ from "jquery";
import CompositionEngine from "./it-ops/CompositionEngine";

$(async () => {

    const engine = new CompositionEngine();

    const modules = ModulesConfig.GetModules();
    for (let i = 0; i < modules.length; i++) {
        const mod = modules[i];
        mod.init(engine, engine);
    }

    window.console.log("Init...");
});