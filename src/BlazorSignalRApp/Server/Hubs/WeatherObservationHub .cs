using Microsoft.AspNetCore.SignalR;
using BlazorSignalRApp.Server.Data;
using BlazorSignalRApp.Shared;

namespace BlazorSignalRApp.Server.Hubs
{
    public class WeatherObservationHub : Hub
    {
        private readonly AppDataContext _db;

        public WeatherObservationHub(AppDataContext db)
        {
            _db = db;
        }
        
        public async Task StoreNewObservation(Observation observation)
        {
            if(observation.Observer == ""||observation.Observer==null)
            {
                await Clients.Caller.SendAsync("InvalidObservationReceived", observation);
            }
            else
            {
                _db.Observations.Add(observation);
                await _db.SaveChangesAsync();
                WeatherForecast weatherForecast = new WeatherForecast
                {
                    Date = observation.Date,
                    TemperatureC = observation.TemperatureC,
                    Summary = observation.Summary
                };
                await Clients.Others.SendAsync("ValidWeatherObservations", weatherForecast);
            }
        }
    }
}
