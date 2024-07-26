using BlazorSignalRApp.Client.Models;
using BlazorSignalRApp.Shared;
using Bunit;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Savonia.xUnit.Helpers;
using Savonia.xUnit.Helpers.Infrastructure;
using System;
using System.Globalization;
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

    public UnitTest(ITestOutputHelper outputHelper, WebApplicationFactoryFixture<Program> factoryFixture) : base(outputHelper)
    {
        _client = factoryFixture.CreateClient();
        _server = factoryFixture.Server;
        WriteLine(factoryFixture.HostUrl);
        _hubUrls = new HubUrls
        {
            ChatHubUrl = $"{factoryFixture.HostUrl}chathub24",
            WeatherHubUrl = $"{factoryFixture.HostUrl}weatherobservations24"
        };
    }

    [Fact]
    public async Task Checkpoint01()
    {
        // Arrange
        System.Random r = new System.Random();
        var auser = $"user-{r.Next()}";
        var amessage = $"message-{r.Next()}";
        var now = DateTime.Now;
        ChatMessage message = new ChatMessage
        {
            User = auser,
            Message = amessage
        };
        ChatMessageNotification notification = new ChatMessageNotification();

        var connection = new HubConnectionBuilder()
            .WithUrl(
                _hubUrls.ChatHubUrl,
                o => o.HttpMessageHandlerFactory = _ => _server.CreateHandler())
            .Build();
        connection.On<ChatMessageNotification>("ChatMessageArrivedNotification", msg =>
        {
            notification = msg;
        });

        await connection.StartAsync();

        // Act
        await Task.Delay(5); // wait some time to make a difference to the message time
        await connection.InvokeAsync("SubmitChatMessage", now, message);
        var response = await _client.GetAsync($"/appdata/messages");

        // Assert
        Assert.Equal(message.User, notification.User);
        Assert.Equal(message.Message, notification.Message);
        Assert.Equal(now, notification.MessageTime, TimeSpan.FromSeconds(1));

        // check that the API returns the same data
        Assert.True(response.IsSuccessStatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains(auser, content);
        Assert.Contains(amessage, content);
        Assert.Contains(now.ToString("yyyy-MM-ddTHH':'mm':'ss"), content);
    }
}