declare var $: any;
import Config from "./Config";
import { httpGetRequest } from "./http";

$(async () => {
    window.console.log("Init...", Config.gatewayBaseUrl);

    const data = await httpGetRequest<any>(Config.gatewayBaseUrl + "/orders");

    window.console.log("data: ", data);
});