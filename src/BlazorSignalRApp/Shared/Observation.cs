using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSignalRApp.Shared
{
    public class Observation : WeatherForecast
    {
        public Guid Id { get; set; }
        public string? ObservationText { get; set; }
        [Required]
        public string Observer { get; set; } = null!; 
    }
}
