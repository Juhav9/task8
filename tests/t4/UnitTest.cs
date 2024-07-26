using BlazorSignalRApp.Client.Models;
using BlazorSignalRApp.Shared;
using Bunit;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Savonia.xUnit.Helpers;
using Savonia.xUnit.Helpers.Infrastructure;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace tests;

public class UnitTest : AppTestBase, IClassFixture<WebApplicationFactoryFixture<Program>>
{
    private readonly HttpClient _client;
    private readonly TestServer _server;
    private readonly HubUrls _hubUrls;
    private readonly Random _random;

    public UnitTest(ITestOutputHelper testOutputHelper, WebApplicationFactoryFixture<Program> factoryFixture) : base(testOutputHelper)
    {
        _client = factoryFixture.CreateClient();
        _server = factoryFixture.Server;
        WriteLine($"host url: {factoryFixture.HostUrl}");
        _hubUrls = new HubUrls
        {
            ChatHubUrl = $"{factoryFixture.HostUrl}chathub24",
            WeatherHubUrl = $"{factoryFixture.HostUrl}weatherobservations24"
        };
        _random = new Random();
    }

    [Fact]
    public async Task Checkpoint04_ValidObservation()
    {
        // Arrange
        Observation observation = new Observation
        {
            Date = System.DateTime.Now,
            ObservationText = $"observation-text-{_random.Next()}",
            TemperatureC = _random.Next(),
            Summary = $"summary-{_random.Next()}",
            Observer = $"observer-{_random.Next()}"
        };
        WeatherForecast forecast = new WeatherForecast();
        WeatherForecast forecast2 = new WeatherForecast();

        var connection = new HubConnectionBuilder()
            .WithUrl(
                _hubUrls.WeatherHubUrl,
                o => o.HttpMessageHandlerFactory = _ => _server.CreateHandler())
            .Build();
        connection.On<WeatherForecast>("ValidWeatherObservations", msg =>
        {
            forecast = msg;
        });

        await connection.StartAsync();

        var connection2 = new HubConnectionBuilder()
            .WithUrl(
                _hubUrls.WeatherHubUrl,
                o => o.HttpMessageHandlerFactory = _ => _server.CreateHandler())
            .Build();
        connection2.On<WeatherForecast>("ValidWeatherObservations", msg =>
        {
            forecast2 = msg;
        });

        await connection2.StartAsync();

        // Act
        await connection.InvokeAsync("StoreNewObservation", observation);
        var response = await _client.GetAsync($"/appdata/weatherobservations");

        // Assert
        Assert.Null(forecast.Summary);
        Assert.Equal(default(int), forecast.TemperatureC);
        Assert.Equal(default(System.DateTime), forecast.Date);
        Assert.Equal(observation.TemperatureC, forecast2.TemperatureC);
        Assert.Equal(observation.Date, forecast2.Date);
        Assert.True(response.IsSuccessStatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains(observation.ObservationText, content);
        Assert.Contains(observation.Summary, content);
    }

    [Fact]
    public async Task Checkpoint04_InvalidObservation()
    {
        // Arrange
        Observation observation = new Observation
        {
            Date = System.DateTime.Now,
            ObservationText = $"observation-text-{_random.Next()}",
            TemperatureC = _random.Next(),
            Summary = $"summary-{_random.Next()}",
            Observer = ""
        };
        WeatherForecast forecast = new WeatherForecast();
        WeatherForecast forecast2 = new WeatherForecast();
        Observation invalid = new Observation();

        var connection = new HubConnectionBuilder()
            .WithUrl(
                _hubUrls.WeatherHubUrl,
                o => o.HttpMessageHandlerFactory = _ => _server.CreateHandler())
            .Build();
        connection.On<WeatherForecast>("ValidWeatherObservations", msg =>
        {
            forecast = msg;
        });
        connection.On<Observation>("InvalidObservationReceived", msg =>
        {
            invalid = msg;
        });

        await connection.StartAsync();

        var connection2 = new HubConnectionBuilder()
            .WithUrl(
                _hubUrls.WeatherHubUrl,
                o => o.HttpMessageHandlerFactory = _ => _server.CreateHandler())
            .Build();
        connection2.On<WeatherForecast>("ValidWeatherObservations", msg =>
        {
            forecast2 = msg;
        });

        await connection2.StartAsync();

        // Act
        await connection.InvokeAsync("StoreNewObservation", observation);
        var response = await _client.GetAsync($"/appdata/weatherobservations");

        // Assert
        Assert.Null(forecast.Summary);
        Assert.Equal(default(int), forecast.TemperatureC);
        Assert.Equal(default(System.DateTime), forecast.Date);
        Assert.Null(forecast2.Summary);
        Assert.Equal(default(int), forecast2.TemperatureC);
        Assert.Equal(default(System.DateTime), forecast2.Date);

        Assert.Equal(observation.Date, invalid.Date);
        Assert.Equal(observation.Summary, invalid.Summary);
        Assert.Equal(observation.ObservationText, invalid.ObservationText);
        Assert.Equal(observation.Observer, invalid.Observer);

        Assert.True(response.IsSuccessStatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain(observation.ObservationText, content);
        Assert.DoesNotContain(observation.Summary, content);
    }

}