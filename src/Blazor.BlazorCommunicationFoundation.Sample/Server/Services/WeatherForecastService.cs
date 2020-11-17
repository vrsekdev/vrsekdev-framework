using Blazor.BlazorCommunicationFoundation.Sample.Shared;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.BlazorCommunicationFoundation.Sample.Server.Services
{
    public class WeatherForecastService : IWeatherForecastContract
    {
        private static readonly string[] Summaries = new[]
{
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [Authorize]
        public Task AddAsync(WeatherForecast weatherForecast)
        {
            return Task.FromResult(0);
        }

        public Task<WeatherForecast[]> GetAsync(WeatherForecastGetFilter filter)
        {
            var rng = new Random();
            return Task.FromResult(Enumerable.Range(1, filter.Count).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray());
        }

        public Task<WeatherForecast[]> GetAsync()
        {
            var rng = new Random();
            return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray());
        }
    }
}
