import * as $ from 'jquery'
import { httpGetRequest } from './http'

$(() => {
    $('#who').keyup(() => {
        sayWho($('#who').val())
    })
    sayWho($('#who').val())
})
type Who = {
    Firstname: string,
    Lastname: string,
    Singlename: string,
    HasSinglename: boolean
}

function sayWho(who: string) {
    const url = '/home/who/' + who
    httpGetRequest<Who>(
        url,
        (e, data) => {
            if (e) return $('#message').text(`error: ${e}`)
            if (data === null) return $('#message').text(`[ERROR: NO DATA]`)
            if (data.HasSinglename) return $('#message').text(`Hello ${data.Singlename}!`)
            return $('#message').text(`Hello Mr. ${data.Firstname} ${data.Lastname}!`)
        }
    )
}
