import * as $ from 'jquery'
export function httpGetRequest<T>(url: string, complete: (error: string | null, data: T | null) => void): any {
    $.ajax(url, {
        method: 'GET',
        success: (data: T, _1: string, _2: any) => complete(null, data),
        error: (_1: any, _2: string, errorThrown: string) => complete(errorThrown, null)
    })
}
