using BlazorSignalRApp.Client.Models;
using BlazorSignalRApp.Client.Pages;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Savonia.xUnit.Helpers;
using Savonia.xUnit.Helpers.Infrastructure;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace test;

public class UnitTest : AppTestBase, IClassFixture<WebApplicationFactoryFixture<Program>>
{
    private readonly HttpClient _client;
    private readonly HubUrls _hubUrls;

    public UnitTest(ITestOutputHelper testOutputHelper, WebApplicationFactoryFixture<Program> factoryFixture) : base(testOutputHelper)
    {
        factoryFixture.HostUrl = "http://localhost:7042/";
        _client = factoryFixture.CreateClient();
        WriteLine($"host url: {factoryFixture.HostUrl}");
        _hubUrls = new HubUrls
        {
            ChatHubUrl = $"{factoryFixture.HostUrl}chathub24",
            WeatherHubUrl = $"{factoryFixture.HostUrl}weatherobservations24"
        };
    }

    [Fact]
    public async Task Checkpoint02()
    {
        // Arrange
        System.Random r = new System.Random();
        var auser = $"user-{r.Next()}";
        var amessage = $"message-{r.Next()}";
        TestContext ctx = new TestContext();
        ctx.Services.AddSingleton(_hubUrls);
        TestContext ctx2 = new TestContext();
        ctx2.Services.AddSingleton(_hubUrls);


        // Act
        var cut = ctx.RenderComponent<Chat>();
        cut.WaitForState(() => cut.FindAll("#submitthemessage").Count.Equals(1));
        var cut2 = ctx2.RenderComponent<Chat>();
        cut2.WaitForState(() => cut2.FindAll("#submitthemessage").Count.Equals(1));
        
        var markup = cut.Markup;
        var userInput = cut.Find("#username");
        var messageInput = cut.Find("#themessage");
        var submitButton = cut.Find("#submitthemessage");

        // Assert
        Assert.NotNull(userInput);
        Assert.NotNull(messageInput);
        Assert.NotNull(submitButton);

        // Act 2
        userInput.Change(auser);
        messageInput.Change(amessage);
        await submitButton.ClickAsync(new MouseEventArgs());

        var div = cut.Find("#messagelist");
        // wait for the send message to render to the sender's UI and others' UIs
        cut.WaitForState(() => div.ChildElementCount > 0);
        var div2 = cut2.Find("#messagelist");
        cut2.WaitForState(() => div2.ChildElementCount > 0);

        // read the message from API
        var response = await _client.GetAsync($"/appdata/messages");

        // Assert 2
        Assert.Contains(auser, div.InnerHtml);
        Assert.Contains(amessage, div.InnerHtml);
        Assert.True(response.IsSuccessStatusCode);
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains(auser, content);
        Assert.Contains(amessage, content);

        Assert.Contains(auser, div2.InnerHtml);
        Assert.Contains(amessage, div2.InnerHtml);
    }
}
