using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherApi.v1.Model
{
    public class DeviceData
    {
        public DateTime Date { get; set; }
        public float TemperatureC { get; set; }
        public float TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        public float Humidity { get; set; }
        public float Rainfall { get; set; }
    }
}
