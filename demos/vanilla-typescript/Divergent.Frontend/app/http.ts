import * as $ from "jquery";

export async function httpGetRequest<T>(url: string): Promise<T> {

    const promise = new Promise<T>((resolve, reject) => {
        $.ajax(url, {
            method: "GET",
            success: (data: T, _1: string, _2: any) => resolve(data),
            error: (_1: any, _2: string, errorThrown: string) => reject(errorThrown)
        });
    });

    return promise;
}
