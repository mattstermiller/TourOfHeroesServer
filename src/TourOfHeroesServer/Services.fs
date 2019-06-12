namespace TourOfHeroesServer

open System
open System.Collections.Generic
open System.Collections.Concurrent

type HeroRepo() =
    let heroes = 
        [   { Id = 11; Name = "Dr Nice" }
            { Id = 12; Name = "Narco" }
            { Id = 13; Name = "Bombasto" }
            { Id = 14; Name = "Celeritas" }
            { Id = 15; Name = "Magneta" }
            { Id = 16; Name = "RubberMan" }
            { Id = 17; Name = "Dynama" }
            { Id = 18; Name = "Dr IQ" }
            { Id = 19; Name = "Magma" }
            { Id = 20; Name = "Tornado" }
        ] |> Seq.map (fun h -> KeyValuePair(h.Id, h)) |> ConcurrentDictionary

    let mutable nextId =
        let keys = heroes.Keys
        if keys.Count = 0 then 0 else Seq.max keys + 1
    let addLock = Object()

    member this.GetAll () =
        heroes.Values |> Seq.sortBy (fun h -> h.Name)

    member this.Get heroId =
        match heroes.TryGetValue heroId with
        | true, hero -> Some hero
        | _ -> None

    member this.Add hero =
        lock addLock (fun () ->
            let hero = { hero with Id = nextId }
            let success = heroes.TryAdd(hero.Id, hero)
            nextId <- nextId + 1
            if success then Some hero else None
        )

    member this.Update hero =
        match this.Get hero.Id with
        | Some existing -> heroes.TryUpdate(hero.Id, hero, existing)
        | None -> false

    member this.Delete heroId =
        heroes.TryRemove(heroId) |> fst

    member this.Search (term: string) =
        let term = term.Trim()
        if term  = "" then
            Seq.empty
        else
            this.GetAll() |> Seq.filter (fun h -> h.Name.Contains(term, StringComparison.CurrentCultureIgnoreCase))
