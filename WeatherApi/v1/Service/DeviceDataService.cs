using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherApi.v1.Model;
using WeatherApi.v1.Repository.Interface;
using WeatherApi.v1.Service.Interface;

namespace WeatherApi.v1.Service
{
    public class DeviceDataService : IDeviceDataService
    {
        private ICloudContainer repository;
        private readonly ILogger<DeviceDataService> logger;

        public DeviceDataService(ICloudContainer repository, ILogger<DeviceDataService> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        public async Task<IDictionary<string, IEnumerable<CsvContent>>> GetDeviceDataAsync(string deviceId, string date, string sensorType = "")
        {
            if (!DateTime.TryParse(date, out _) || !SensorType.IsValidType(sensorType))
                return new Dictionary<string, IEnumerable<CsvContent>>();
            try
            {
                return await repository.GetItemsAsync(deviceId, date, sensorType);
            }
            catch (Exception ex)
            {
                logger.LogError("Repository error", ex);
                return new Dictionary<string, IEnumerable<CsvContent>>();
            }
        }
    }
}
