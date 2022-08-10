using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherApi.v1.Model;

namespace WeatherApi.v1.Service.Interface
{
    public interface IDeviceDataService
    {
        Task<IDictionary<string, IEnumerable<CsvContent>>> GetDeviceDataAsync(string deviceId, string date, string sensorType = "");
            
    }
}
