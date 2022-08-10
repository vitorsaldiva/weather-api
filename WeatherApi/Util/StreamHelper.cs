using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using WeatherApi.v1.Model;

namespace WeatherApi.Util
{
    public class StreamHelper
    {
        public static IEnumerable<CsvContent> GetCsvData(Stream fileStream, CultureInfo cultureInfo)
        {
            var config = new CsvConfiguration(cultureInfo)
            {
                Delimiter = ";",
            };
            using (var streamReader = new StreamReader(fileStream))
            using (var csvReader = new CsvReader(streamReader, config))
            {
                return csvReader.GetRecords<CsvContent>().ToList();
            }
        }
    }
}
