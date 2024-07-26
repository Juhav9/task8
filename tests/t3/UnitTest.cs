using BlazorSignalRApp.Server.Data;
using BlazorSignalRApp.Shared;
using Bunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Savonia.xUnit.Helpers;
using Savonia.xUnit.Helpers.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace tests;

public class UnitTest : AppTestBase, IClassFixture<WebApplicationFactoryFixture<Program>>, IClassFixture<TestContext>
{
    private readonly HttpClient _client;
    private readonly AppDataContext _db;
    private readonly Random _random;
    public UnitTest(ITestOutputHelper testOutputHelper, WebApplicationFactoryFixture<Program> factoryFixture, TestContext context) : base(testOutputHelper)
    {
        _client = factoryFixture.CreateDefaultClient();
        _db = factoryFixture.Services.CreateScope().ServiceProvider.GetRequiredService<AppDataContext>();
        _random = new Random();
    }

    [Fact]
    public void Checkpoint03_1()
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

        // Act

        // Assert
        Assert.IsAssignableFrom<WeatherForecast>(observation);
    }

    [Fact]
    public async Task Checkpoint03_2()
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

        // Act
        _db.Add(observation);
        var result = await _db.SaveChangesAsync();
        var o = await _db.Observations.FirstOrDefaultAsync();

        // Assert
        Assert.Equal(1, result);
        Assert.NotNull(o);
        Assert.Equal(observation.ObservationText, o.ObservationText);
        Assert.Equal(observation.Observer, o.Observer);
        Assert.Equal(observation.TemperatureC, o.TemperatureC);
    }

    [Theory]
    [JsonFileData("testdata.json", typeof(Tuple<string, Observation>), typeof(Observation))]
    public async Task Checkpoint03_3(Tuple<string, Observation> data, Observation expected)
    {
        // Arrange
        
        // Act
        _db.Add(data.Item2);
        var result = await _db.SaveChangesAsync();
        // Act
        var response = await _client.GetAsync(data.Item1);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        var content = await response.Content.ReadFromJsonAsync<IEnumerable<Observation>>();
        Assert.NotNull(content);
        Assert.NotEmpty(content);
        Assert.Contains(expected, content, new ObservationComparer());
    }
}

public class ObservationComparer : IEqualityComparer<Observation>
{
    public bool Equals(Observation? x, Observation? y)
    {
        if (x == null || y == null)
        {
            return false;
        }
        return x.Observer == y.Observer && x.TemperatureC == y.TemperatureC && x.Date == y.Date && x.ObservationText == y.ObservationText && x.Summary == y.Summary;
    }

    public int GetHashCode([DisallowNull] Observation obj)
    {
        return obj.Observer.GetHashCode() ^ obj.TemperatureC.GetHashCode() ^ obj.Date.GetHashCode() ^ (obj.ObservationText?.GetHashCode() ?? 1) ^ (obj.Summary?.GetHashCode() ?? 2);
    }
}