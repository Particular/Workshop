import { IViewModelAppender } from "../../it-ops/IViewModelAppender";
import { httpGetRequest } from "../../http";

export default class OrderDetailsAppender implements IViewModelAppender {

    get requestIdentifier(): string {
        return "Order/Details";
    }

    async append(viewModel: any, requestArgs: any): Promise<void> {

        const id = requestArgs.id;
        const data = await httpGetRequest<any>("http://localhost:20395/api/orders/" + id);

        viewModel.orderNumber = data.number;
        viewModel.orderItemsCount = data.itemsCount;
    }
}