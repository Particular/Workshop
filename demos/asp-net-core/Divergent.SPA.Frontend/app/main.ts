declare var $: any;
import Config from "./Config";
import { httpGetRequest } from "./http";

$(async () => {
    const data = await httpGetRequest<any>(Config.gatewayBaseUrl + "/orders");
    const orders: any[] = data.orders;

    $("#app").empty();
    const appContainer: HTMLElement = $("#app");

    window.console.log("appContainer", appContainer);
    window.console.log("Data", data);
    window.console.log("orders", orders);
    
    for (var i = 0; i < orders.length; i++) {
        const o = orders[i];
        window.console.log("order", o);
    }
});