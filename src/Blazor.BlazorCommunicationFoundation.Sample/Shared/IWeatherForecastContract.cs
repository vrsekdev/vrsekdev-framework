using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.BlazorCommunicationFoundation.Sample.Shared
{
    public interface IWeatherForecastContract
    {
        Task<WeatherForecast[]> GetAsync();

        Task<WeatherForecast[]> GetAsync(WeatherForecastGetFilter filter);

        Task AddAsync(WeatherForecast weatherForecast);
    }
}
