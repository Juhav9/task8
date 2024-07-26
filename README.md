[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-24ddc0f5d75046c5622901739e7c5dd533143b0c8e959d652212380cedb1ea36.svg)](https://classroom.github.com/a/CG8A7eX6)
# Task 08: SignalR

<img alt="points bar" align="right" height="36" src="../../blob/badges/.github/badges/points-bar.svg" />

![GitHub Classroom Workflow](../../workflows/GitHub%20Classroom%20Workflow/badge.svg)

***

## Student info

> Write your name, your estimation of how many points you will get, and an estimate of how long it took to make the answer

- Student name: 
- Estimated points: 
- Estimated time (hours): 

***

## Purpose of this task

The purposes of this task are:

- to learn to create real-time apps
- to learn to use SignalR
- to learn to create a Blazor client app for SignalR communication

## Material for the task

> **Following material will help with the task.**

It is recommended that you will check the material before start coding.

1. [Overview of ASP.NET Core SignalR](https://learn.microsoft.com/en-gb/aspnet/core/signalr/introduction?view=aspnetcore-6.0)
2. [Use ASP.NET Core SignalR with Blazor](https://learn.microsoft.com/en-gb/aspnet/core/blazor/tutorials/signalr-blazor?view=aspnetcore-6.0&tabs=visual-studio&pivots=webassembly)
3. [ASP.NET Core Blazor forms and input components](https://learn.microsoft.com/en-us/aspnet/core/blazor/forms-and-input-components?view=aspnetcore-6.0#built-in-input-components)

## The Task

Create a SignalR application with Blazor WebAssembly client. The template with some initial data is given in BlazorSignalRApp. Your task is to complete the app. The app has two SignalR hubs. One for chat messages and one for weather observation data. Some of the needed model classes are given and some needs to be implemented. The app also has a web api controller for reading the saved data from the in-memory database.

The application uses in-memory database for storing the chat messages and the weather observations. The database context and configuration is already done.

What is given:

- ChatMessage class
- ChatMessageNotification class
- WeatherForecast class
- HubUrls class
- Chat.razor file, which
  - has the url
- Observations.razor file
- In-memory partial data context (`AppDataContext`) for chat messages and weather observations

What needs to be done (and this is a long long list...) is described in detail in the following five steps in the evaluation points chapter. In short you will need to implement an endpoint to the web api, SignalR hubs to the server, user interface for the hubs as Blazor pages in the client and complete the data context class.

> **Note!** The src/BlazorSignalRApp folder contains a hosted BlazorWASM app. To run the whole app run the Server project. That will start both the server and the client. More info in src/README.md file.

### Evaluation points

1. Create a hub named `ChatterHub` for chat messages. When a message is send to ChatterHub it is saved to db and then transmitted to **all** clients (including the message sender).

    The ChatterHub:

    - Has a method named `SubmitChatMessage` which takes two parameters. The first parameter is of type `DateTime` where the sender's current time is transmitted and the second parameter is of type `ChatMessage` where the actual message info is transmitted.
    - The incoming message is saved to in-memory database as type `ChatMessageNotification`. The sender's time is used as the message's time when saving to the database. 
    - The SubmitChatMessage method transmits the received message to all clients as type `ChatMessageNotification` by calling `ChatMessageArrivedNotification` method on clients.
    - Is in `BlazorSignalRApp.Server.Hubs` namespace
    - Has only one constructor, which takes the AppDataContext as a parameter
      ```csharp
        public ChatterHub(AppDataContext db)
        {
          // ...
        }
      ```
    - Is accessible from URI: `/chathub24`

2. Create a client for the ChatHub to the Client app as Blazor page named `Chat.razor`.

    The Chat page:

    - Has url `/chat` (i.e. the page is accessible from url /chat)
    - Has inputs for user's name (`id="username"`) and the message (`id="themessage"`). Use the given id values for the input fields.
    - Has only one button to submit the message. Use `id="submitthemessage"` for the button. Clicking the button sends the message via SignalR to the ChatHub. The button must not be rendered on the page before the SignalR connection is established.
    - Lists received messages as div elements with css classes: `alert` `alert-success` `messagenotification`. The div element contains the message's MessageTime, User and Message properties' values. The div elements are listed in a div element with `id="messagelist"`.
    - Use value from injected HubUrls class' ChatHubUrl property as the hub's url

3. Add a DbSet property to `AppDataContext` class in folder Server/Data to support storing weather observations. Use the name `Observations` for the DbSet property. Complete the implementation of web api endpoints for app data on `Server/Controllers/AppDataController` class.

    The Observation class:

    - Create a weather observation class named `Observation`, which:
       - Inherits `WeatherForecast` class
       - Is in namespace `BlazorSignalRApp.Shared`
       - Is in project `BlazorSignalRApp.Shared`
       - Has additional properties:
         - Guid Id (i.e. property name is Id and its type is Guid)
         - string? ObservationText (i.e. nullable string)
         - string Observer (i.e. who submitted the observation). Decorate with `Required` attribute.

4. Create a hub named `WeatherObservationHub` for weather observations. When an observation is submitted to WeatherObservationHub it is saved to db and then transmitted to all **other** clients (excluding the message sender).

    The WeatherObservationHub:

    - Has a method named `StoreNewObservation` to store submitted weather observations as `Observation` class instance
    - The incoming observation validity is checked (i.e. all properties that are marked as required are populated)
    - Implement the validation logic in the StoreNewObservation method
    - If the validation fails (i.e. required properties are not populated (=are null or empty)) then nothing is saved to db and nothing is transmitted to other clients. The invalid data (the Observation class instance) is transmitted to the caller itself via calling its `InvalidObservationReceived` method.
    - The valid incoming observation is save to in-memory database
    - The StoreNewObservation method transmits the valid observation to all other clients as `WeatherForecast` class instance by calling `ValidWeatherObservations` method on the clients.
    - Is in `BlazorSignalRApp.Server.Hubs` namespace
    - Has only one constructor, which takes the AppDataContext as a parameter
      ```csharp
        public WeatherObservationHub(AppDataContext db)
        {
          // ...
        }
      ```
    - Is accessible from URI: `/weatherobservations24`

5. Create a client for the WeatherHub to the Client app as Blazor page named `Observations.razor`.

    The Weather page:

    - Has url `/observations`
    - Has inputs for:
      - Date, use InputDate control
      - TemperatureC, use InputNumber control
      - Summary, use InputText control with attribute `id="summary"`
      - ObservationText, use InputText control with attribute `id="observationtext"`
      - Observer, use InputText control with attribute `id="observer"`
    - Has only one button to submit the observation `id="submit"`. Clicking the button sends the message via SignalR to the WeatherObservationHub. The button must not be rendered on the page before the SignalR connection is established.
    - Lists all received valid observations as rows in a `table` element where WeatherForecast class' properties' Date, TemperatureC, TemperatureF and Summary values are in their own columns. The table has headers.
    - Lists all invalid observations as `div` elements with css class values `alert` `alert-danger` `invalid-observation`. Each div contains values for all readable properties of the invalid observation.

    > **Note about client side validation!** Do **NOT** validate on the client side. The invalid values are meant to go through the server. The server decides what to do with the received data. (i.e. in some `EditForm` samples there is `OnValidSubmit` event used but in this task do not use that, select other ways to handle the submit button).

> **Note!** Read the task description and the evaluation points to get the task's specification (what is required to make the app complete).

> **NOTE!** Do NOT update the NuGet package references on either the client or the server apps.


## Task evaluation

Evaluation points for the task are described above. An evaluation point either works or does not work there is no "it kind of works" step in between. Be sure to test your work. All working evaluation points are added to the task total and will count toward the course total. The task is worth five (5) points. Each evaluation point is checked individually and each will provide one (1) point so there are five checkpoints. Checkpoints are designed so that they may require additional code, that is not checked or tested, to function correctly.

## DevOps

There is a DevOps pipeline added to this task. The pipeline will build the solution and run automated tests on it. The pipeline triggers when a commit is pushed to GitHub on the main branch. So remember to `git commit` and `git push` when you are ready with the task. The automation uses GitHub Actions and some task runners. The automation is in the folder named .github.

> **DO NOT modify the contents of .github or tests folders.**
