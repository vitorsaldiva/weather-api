using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using WeatherApi.v1.Model;
using WeatherApi.v1.Repository.Interface;
using WeatherApi.v1.Service;
using WeatherApi.v1.Service.Interface;
using Xunit;

namespace WeatherApi.Tests.Service
{
    public class DeviceDataServiceTests
    {
        private readonly Mock<ICloudContainer> repository;
        private readonly Mock<ILogger<DeviceDataService>> logger;
        private IDeviceDataService service;

        public DeviceDataServiceTests()
        {
            repository = new Mock<ICloudContainer>();
            logger = new Mock<ILogger<DeviceDataService>>();
        }
        [Fact]
        public async void DeviceIdNotFound_ShouldReturn_EmptyCollection()
        {
            //Arrange
            var deviceId = "testData";
            var date = DateTime.Today.ToString("yyyy-MM-dd");
            var sensorType = "humidity";
            repository.Setup(r => r.GetItemsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new Dictionary<string, IEnumerable<CsvContent>>
                {
                });
            
            service = new DeviceDataService(repository: repository.Object, logger.Object);

            //Act
            var response = await service.GetDeviceDataAsync(deviceId, date, sensorType);

            //Assert
            Assert.Empty(response);

        }
        [Fact]
        public async void SensorTypeNotInformed_ShouldReturn_DeviceData()
        {
            //Arrange
            var deviceId = "testData";
            var date = DateTime.Today.ToString("yyyy-MM-dd");
            var sensorType = string.Empty;
            repository.Setup(r => r.GetItemsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new Dictionary<string, IEnumerable<CsvContent>>
                {
                    { SensorType.HUMIDITY , new List<CsvContent>{ new CsvContent { Date = DateTime.Parse("2022-12-01 00:00:30"), Info = 30.5f } } },
                    { SensorType.TEMPERATURE , new List<CsvContent>{ new CsvContent { Date = DateTime.Parse("2022-12-01 00:00:30"), Info = 31.7f } } },
                    { SensorType.RAINFALL , new List<CsvContent>{ new CsvContent { Date = DateTime.Parse("2022-12-01 00:00:30"), Info = 3.9f } } }
                });

            service = new DeviceDataService(repository: repository.Object, logger.Object);

            //Act
            var response = await service.GetDeviceDataAsync(deviceId, date, sensorType);

            //Assert
            Assert.NotEmpty(response);

        }
        [Theory]
        [InlineData("invalidDate")]
        [InlineData("2011-31-12")]
        public async void InvalidDate_ShouldReturn_EmptyCollection(string date)
        {
            //Arrange
            var deviceId = "testData";
            var sensorType = "humidity";
            repository.Setup(r => r.GetItemsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new Dictionary<string, IEnumerable<CsvContent>>
                {
                });

            service = new DeviceDataService(repository: repository.Object, logger.Object);

            //Act
            var response = await service.GetDeviceDataAsync(deviceId, date, sensorType);

            //Assert
            Assert.Empty(response);
            repository.Verify(r => r.GetItemsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
        [Fact]
        public async void ValidDate_ShouldReturn_SensorData()
        {
            //Arrange
            var deviceId = "testData";
            var sensorType = "humidity";
            var date = "2022-12-01";
            repository.Setup(r => r.GetItemsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new Dictionary<string, IEnumerable<CsvContent>>
                {
                    { sensorType, new List<CsvContent>{ new CsvContent { Date = DateTime.Parse("2022-12-01 00:00:30"), Info = 30.5f } } }
                });

            service = new DeviceDataService(repository.Object, logger.Object);

            //Act
            var response = await service.GetDeviceDataAsync(deviceId, date, sensorType);

            //Assert
            Assert.NotEmpty(response);
            repository.Verify(r => r.GetItemsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
        [Fact]
        public async void RepositoryThrowsException_ShouldReturn_EmptyCollection()
        {
            //Arrange
            var deviceId = "testData";
            var sensorType = "humidity";
            var date = DateTime.Today.ToString("yyyy-MM-dd");
            repository.Setup(r => r.GetItemsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception());


            service = new DeviceDataService(repository: repository.Object, logger.Object);

            //Act
            var response = await service.GetDeviceDataAsync(deviceId, date, sensorType);

            //Assert
            Assert.Empty(response);
            repository.Verify(r => r.GetItemsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

    }
}
