module TourOfHeroesServer.HttpHandlers

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2.ContextInsensitive
open Giraffe

type HttpContext with
    member this.GetService<'a> () =
        this.RequestServices.GetService(typeof<'a>) :?> 'a

let private successOrFailHandler action succeeded =
    if succeeded then json (action + " succeeded")
    else setStatusCode 500 >=> json (action + " failed")


let handleGetHeroes next (ctx: HttpContext) = task {
    let response = ctx.GetService<HeroRepo>().GetAll ()
    return! json response next ctx
}

let handleGetHero id next (ctx: HttpContext) = task {
    let handler =
        match ctx.GetService<HeroRepo>().Get id with
        | Some hero -> json hero
        | None -> setStatusCode 404 >=> json "Specified id does not exist"
    return! handler next ctx
}

let handleGetHeroSearch next (ctx: HttpContext) = task {
    let handler =
        match ctx.GetQueryStringValue("name") with
        | Ok searchTerm ->
            ctx.GetService<HeroRepo>().Search searchTerm |> json
        | Error e ->
            setStatusCode 500 >=> json e
    return! handler next ctx
}

let handlePutHero next (ctx: HttpContext) = task {
    let! hero = ctx.BindJsonAsync<Hero>()
    let handler = ctx.GetService<HeroRepo>().Update hero |> successOrFailHandler "Update"
    return! handler next ctx
}

let handlePostHero next (ctx: HttpContext) = task {
    let! hero = ctx.BindJsonAsync<Hero>()
    let handler =
        match ctx.GetService<HeroRepo>().Add hero with
        | Some newHero -> json newHero
        | None -> setStatusCode 500 >=> json "Add failed"
    return! handler next ctx
}

let handleDeleteHero id next (ctx: HttpContext) = task {
    let handler = ctx.GetService<HeroRepo>().Delete id |> successOrFailHandler "Delete"
    return! handler next ctx
}
