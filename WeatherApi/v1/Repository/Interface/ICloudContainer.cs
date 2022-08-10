using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherApi.v1.Model;

namespace WeatherApi.v1.Repository.Interface
{
    public interface ICloudContainer
    {
        Task<IDictionary<string, IEnumerable<CsvContent>>> GetItemsAsync(string deviceId, string date, string sensorType = "");
    }
}
