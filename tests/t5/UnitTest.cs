using BlazorSignalRApp.Client.Models;
using BlazorSignalRApp.Client.Pages;
using BlazorSignalRApp.Shared;
using Bunit;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
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
    private readonly HubUrls _hubUrls;
    private readonly Random _random = new Random();

    public UnitTest(ITestOutputHelper testOutputHelper, WebApplicationFactoryFixture<Program> factoryFixture) : base(testOutputHelper)
    {
        factoryFixture.HostUrl = "http://localhost:7045/";
        _client = factoryFixture.CreateClient();
        _hubUrls = new HubUrls
        {
            ChatHubUrl = $"{factoryFixture.HostUrl}chathub24",
            WeatherHubUrl = $"{factoryFixture.HostUrl}weatherobservations24"
        };
    }

    [Fact]
    public async Task Checkpoint05_ValidObservations()
    {
        // Arrange
        TestContext ctx = new TestContext();
        ctx.Services.AddSingleton(_hubUrls);
        TestContext ctx2 = new TestContext();
        ctx2.Services.AddSingleton(_hubUrls);

        Observation observation = new Observation
        {
            Date = System.DateTime.Now,
            ObservationText = $"observation-text-{_random.Next()}",
            TemperatureC = _random.Next(),
            Summary = $"summary-{_random.Next()}",
            Observer = $"observer-{_random.Next()}"
        };

        // Act
        var cut = ctx.RenderComponent<Observations>();
        var cut2 = ctx2.RenderComponent<Observations>();
        cut.WaitForState(() => cut.FindAll("#submit").Count.Equals(1));
        cut2.WaitForState(() => cut.FindAll("#submit").Count.Equals(1));
        var markup = cut.Markup;

        var dateInput = cut.FindComponent<InputDate<System.DateTime>>().Find("input");
        var tempCInput = cut.FindComponent<InputNumber<int>>().Find("input");
        var summaryInput = cut.Find("#summary");
        var observationInput = cut.Find("#observationtext");
        var observerInput = cut.Find("#observer");
        var submitButton = cut.Find("#submit");

        dateInput.Change(observation.Date);
        tempCInput.Change(observation.TemperatureC);
        summaryInput.Change(observation.Summary);
        observationInput.Change(observation.ObservationText);
        observerInput.Change(observation.Observer);
        await submitButton.ClickAsync(new MouseEventArgs());

        var table = cut.Find("table");
        cut.WaitForState(() => cut.FindAll("tr").Count.Equals(1));
        var table2 = cut2.Find("table");
        cut2.WaitForState(() => cut2.FindAll("tr").Count > 1);

        // read the data from API
        var response = await _client.GetAsync($"/appdata/weatherobservations");

        Assert.True(response.IsSuccessStatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains(observation.TemperatureC.ToString(), content);
        Assert.Contains(observation.ObservationText, content);
        Assert.DoesNotContain(observation.TemperatureC.ToString(), table.InnerHtml);
        Assert.DoesNotContain(observation.Summary, table.InnerHtml);
        Assert.Contains(observation.TemperatureC.ToString(), table2.InnerHtml);
        Assert.Contains(observation.Summary, table2.InnerHtml);
    }

    [Fact]
    public async Task Checkpoint05_InvalidObservation()
    {
        // Arrange
        TestContext ctx = new TestContext();
        ctx.Services.AddSingleton(_hubUrls);
        TestContext ctx2 = new TestContext();
        ctx2.Services.AddSingleton(_hubUrls);

        Observation observation = new Observation
        {
            Date = System.DateTime.Now,
            ObservationText = $"observation-text-{_random.Next()}",
            TemperatureC = _random.Next(),
            Summary = $"summary-{_random.Next()}",
            Observer = $""
        };

        // Act
        var cut = ctx.RenderComponent<Observations>();
        var cut2 = ctx2.RenderComponent<Observations>();
        cut.WaitForState(() => cut.FindAll("#submit").Count.Equals(1));
        cut2.WaitForState(() => cut.FindAll("#submit").Count.Equals(1));
        var markup = cut.Markup;

        var dateInput = cut.FindComponent<InputDate<System.DateTime>>().Find("input");
        var tempCInput = cut.FindComponent<InputNumber<int>>().Find("input");
        var summaryInput = cut.Find("#summary");
        var observationInput = cut.Find("#observationtext");
        var observerInput = cut.Find("#observer");
        var submitButton = cut.Find("#submit");

        dateInput.Change(observation.Date);
        tempCInput.Change(observation.TemperatureC);
        summaryInput.Change(observation.Summary);
        observationInput.Change(observation.ObservationText);
        observerInput.Change(observation.Observer);
        await submitButton.ClickAsync(new MouseEventArgs());

        var table = cut.Find("table");
        cut.WaitForState(() => cut.FindAll("tr").Count.Equals(1));
        var table2 = cut2.Find("table");
        cut2.WaitForState(() => cut2.FindAll("tr").Count.Equals(1));
        
        cut.WaitForState(() => cut.FindAll(".invalid-observation").Count > 0);
        var invalidObservations = cut.FindAll(".invalid-observation");
        // read the data from API
        var response = await _client.GetAsync($"/appdata/weatherobservations");

        Assert.True(response.IsSuccessStatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain(observation.TemperatureC.ToString(), content);
        Assert.DoesNotContain(observation.ObservationText, content);

        Assert.DoesNotContain(observation.TemperatureC.ToString(), table.InnerHtml);
        Assert.DoesNotContain(observation.Summary, table.InnerHtml);
        Assert.DoesNotContain(observation.TemperatureC.ToString(), table2.InnerHtml);
        Assert.DoesNotContain(observation.Summary, table2.InnerHtml);

        Assert.NotNull(invalidObservations);
        Assert.NotEmpty(invalidObservations);
        Assert.Contains(observation.Summary, invalidObservations[0].InnerHtml);
        Assert.Contains(observation.ObservationText, invalidObservations[0].InnerHtml);
        Assert.Contains(observation.TemperatureC.ToString(), invalidObservations[0].InnerHtml);
    }
}