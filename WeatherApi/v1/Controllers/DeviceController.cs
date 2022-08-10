using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherApi.v1.Model;
using WeatherApi.v1.Service.Interface;

namespace WeatherApi.v1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    //[Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/devices")]
    [Route("api/devices")]
    public class DeviceController : ControllerBase
    {
        private ILogger<DeviceController> logger;
        private IDeviceDataService deviceService;

        public DeviceController(ILogger<DeviceController> logger, IDeviceDataService deviceService)
        {
            this.logger = logger;
            this.deviceService = deviceService;
        }

        [HttpGet]
        [Route("{deviceId}/data/{date}")]
        public async Task<IDictionary<string, IEnumerable<CsvContent>>> GetDeviceDataAsync(string deviceId, string date)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentException($"'{nameof(deviceId)}' cannot be null or empty.", nameof(deviceId));
            }

            if (string.IsNullOrEmpty(date))
            {
                throw new ArgumentException($"'{nameof(date)}' cannot be null or empty.", nameof(date));
            }

            return await deviceService.GetDeviceDataAsync(deviceId, date);
        }

        [HttpGet]
        [Route("{deviceId}/data/{date}/{sensorType}")]
        public async Task<IDictionary<string, IEnumerable<CsvContent>>> GetDeviceDataAsync(string deviceId, string date, string sensorType)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentException($"'{nameof(deviceId)}' cannot be null or empty.", nameof(deviceId));
            }

            if (string.IsNullOrEmpty(date))
            {
                throw new ArgumentException($"'{nameof(date)}' cannot be null or empty.", nameof(date));
            }

            if (string.IsNullOrEmpty(sensorType))
            {
                throw new ArgumentException($"'{nameof(sensorType)}' cannot be null or empty.", nameof(sensorType));
            }

            return await deviceService.GetDeviceDataAsync(deviceId, date, sensorType);
        }
    }
}
