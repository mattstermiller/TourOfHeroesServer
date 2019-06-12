module TourOfHeroesServer.App

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open TourOfHeroesServer.HttpHandlers

let webApp =
    choose [
        GET >=> choose [
            routex "/heroes" >=> handleGetHeroes
            routex "/heroes/" >=> handleGetHeroSearch
            routef "/heroes/%i" handleGetHero
        ]
        PUT >=> choose [
            route "/heroes" >=> handlePutHero
        ]
        POST >=> choose [
            route "/heroes" >=> handlePostHero
        ]
        DELETE >=> choose [
            routef "/heroes/%i" handleDeleteHero
        ]
        setStatusCode 404 >=> text "Not Found"
    ]

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

let configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins("http://localhost:4200")
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowAnyOrigin()
           |> ignore

let configureApp (app : IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IHostingEnvironment>()
    let app =
        if env.IsDevelopment() then
            app.UseDeveloperExceptionPage()
        else
            app.UseHttpsRedirection()
               .UseGiraffeErrorHandler errorHandler
    app.UseCors(configureCors)
       .UseGiraffe(webApp)

let configureServices (services : IServiceCollection) =
    services.AddSingleton(HeroRepo())
            .AddCors()
            .AddGiraffe()
    |> ignore

let configureLogging (builder : ILoggingBuilder) =
    builder.AddFilter(fun l -> l.Equals LogLevel.Error)
           .AddConsole()
           .AddDebug()
   |> ignore

[<EntryPoint>]
let main _ =
    WebHostBuilder()
        .UseKestrel()
        .UseIISIntegration()
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(configureServices)
        .ConfigureLogging(configureLogging)
        .Build()
        .Run()
    0