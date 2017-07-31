declare var $: any;
import Config from "./Config";
import { httpGetRequest } from "./http";

$(async () => {
    const data = await httpGetRequest<any>(Config.gatewayBaseUrl + "/orders");
    const orders: any[] = data.orders;

    for (var i = 0; i < orders.length; i++) {
        const o = orders[i];

        $(".orders-table > tbody").append("<tr class=\"order-row-" + i + "\"></tr>");
        $(".order-row-" + i).append("<td>" + o.orderNumber + "</td>");
        $(".order-row-" + i).append("<td>" + o.orderItemsCount + "</td>");
        $(".order-row-" + i).append("<td>" + o.shippingCourier + "</td>");
        $(".order-row-" + i).append("<td>" + o.shippingStatus + "</td>");
    }

    $(".orders").show();
});