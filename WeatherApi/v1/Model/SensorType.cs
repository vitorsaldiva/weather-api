using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherApi.v1.Model
{
    public static class SensorType
    {
        public const string HUMIDITY = "humidity";
        public const string RAINFALL = "rainfall";
        public const string TEMPERATURE = "temperature";
        public static bool IsValidType(string sensor)
        {
            return sensor.ToLower().Equals(HUMIDITY) || sensor.ToLower().Equals(RAINFALL) ||
                sensor.ToLower().Equals(TEMPERATURE) || string.IsNullOrEmpty(sensor);
        }
    }

}
