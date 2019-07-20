# Tour Of Heroes Server

This project is a server for the Angular tutorial project [Tour Of Heroes](https://angular.io/tutorial) and can replace the mock server used in the tutorial.

You can start the web server by executing the following command in your terminal:

```
dotnet run -p src/TourOfHeroesServer
```

After the application has started, the API will respond to requests at
[http://localhost:5000/heroes](http://localhost:5000/heroes) and
[https://localhost:5001/heroes](https://localhost:5001/heroes).
It allows requests from http://localhost:4200 and http://localhost:3000.
