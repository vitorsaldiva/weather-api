using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using WeatherApi.Util;
using WeatherApi.v1.Model;
using WeatherApi.v1.Repository.Interface;

namespace WeatherApi.v1.Repository
{
    public class CloudContainer : ICloudContainer
    {
        private readonly string connectionString = "";
        private readonly string containerName = "";
        private readonly string cultureName = "";
        private readonly IConfiguration configuration;

        public CloudContainer(IConfiguration configuration)
        {
            this.configuration = configuration;
            connectionString = this.configuration.GetSection("AzureStorage:ConnectionString").Value;
            containerName = this.configuration.GetSection("AzureStorage:ContainerName").Value;
            cultureName = this.configuration.GetSection("Localization:Culture").Value;
        }

        public async Task<IDictionary<string, IEnumerable<CsvContent>>> GetItemsAsync(string deviceId, string date, string sensorType = "")
        {
            var container = new BlobContainerClient(connectionString, containerName);

            Func<BlobItem, bool> func = (BlobItem blob) => blob.Name.Contains(deviceId) && blob.Name.Contains(date);
            if (!string.IsNullOrEmpty(sensorType))
                func = (BlobItem blob) => blob.Name.Contains(deviceId) && blob.Name.Contains(date) &&
                                            blob.Name.Contains(sensorType);

            IDictionary<string, IEnumerable<CsvContent>> response = new Dictionary<string, IEnumerable<CsvContent>>();
            foreach (var item in container.GetBlobs().Where(func))
            {
                var sensor = GetSensorType(item);
                var blobService = container.GetBlobClient(item.Name);
                var stream = await blobService.OpenReadAsync();
                response.Add(sensor, StreamHelper.GetCsvData(stream, cultureInfo: CultureInfo.GetCultureInfo(cultureName)));
            }

            if (response is null || response.Count().Equals(0))
                response = await GetHistoricalDataAsync(deviceId, date, sensorType);

            return response;
        }

        private static string GetSensorType(BlobItem item)
        {
            return item.Name.Split("/")[1];
        }

        private async Task<IDictionary<string, IEnumerable<CsvContent>>> GetHistoricalDataAsync(string deviceId, string date, string sensorType = "")
        {
            var response = new Dictionary<string, IEnumerable<CsvContent>>();
            var client = new BlobServiceClient(connectionString);
            var container = client.GetBlobContainerClient(containerName);

            if (string.IsNullOrEmpty(sensorType))
            {
                foreach (var item in container.GetBlobs().Where(b => b.Name.Contains(deviceId) && b.Name.Contains("historical")))
                {
                    var blobClient = container.GetBlobClient(item.Name);
                    var stream = await blobClient.OpenReadAsync();
                    var sensor = GetSensorType(item);
                    using var package = new ZipArchive(stream, ZipArchiveMode.Read);
                    var fileToBeOpened = package.Entries.Where((ZipArchiveEntry z) => z.FullName.Contains(date))
                            .Select(b => b.FullName).FirstOrDefault();
                    var fileStream = package.GetEntry(fileToBeOpened).Open();
                    response.Add(sensor, StreamHelper.GetCsvData(fileStream, CultureInfo.GetCultureInfo(cultureName)));
                }
            }
            else
            {
                var historicalFile = container.GetBlobs().Where(blob => blob.Name.Contains(deviceId) &&
                                            blob.Name.Contains(sensorType) && blob.Name.Contains("historical"))
                                    .Select(b => b.Name).FirstOrDefault();

                var blobClient = container.GetBlobClient(historicalFile);
                var stream = await blobClient.OpenReadAsync();
                using var package = new ZipArchive(stream, ZipArchiveMode.Read);
                var fileToBeOpened = package.Entries.Where((ZipArchiveEntry z) => z.FullName.Contains(date))
                        .Select(b => b.FullName).FirstOrDefault();
                var fileStream = package.GetEntry(fileToBeOpened).Open();
                response.Add(sensorType, StreamHelper.GetCsvData(fileStream, CultureInfo.GetCultureInfo(cultureName)));
            }

            return response;
        }
    }
}
