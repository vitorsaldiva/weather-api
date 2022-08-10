using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherApi.v1.Model
{
    public class CsvContent
    {
        [Index(0)]
        public DateTime Date { get; set; }
        [Index(1)]
        public float Info { get; set; }
    }
}
