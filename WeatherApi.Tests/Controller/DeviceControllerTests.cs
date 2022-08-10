using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeatherApi.v1.Controllers;
using WeatherApi.v1.Model;
using WeatherApi.v1.Service.Interface;
using Xunit;

namespace WeatherApi.Tests.Controller
{
    public class DeviceControllerTests
    {
        private readonly Mock<ILogger<DeviceController>> logger;
        private readonly Mock<IDeviceDataService> service;

        public DeviceControllerTests()
        {
            logger = new Mock<ILogger<DeviceController>>();
            service = new Mock<IDeviceDataService>();
        }

        [Fact]
        public async void DeviceNotFound_ShouldReturn_EmptyCollection()
        {
            //Arrange
            var deviceController = new DeviceController(logger.Object, service.Object);
            string deviceId = "testDevice";
            var date = DateTime.Today.ToString("yyyy-MM-dd");
            var sensorType = string.Empty;
            service.Setup(s => s.GetDeviceDataAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new Dictionary<string, IEnumerable<CsvContent>>());

            //Act
            var response = await deviceController.GetDeviceDataAsync(deviceId, date);

            //Assert
            Assert.Empty(response);
        }
        [Fact]
        public async void FutureDate_ShouldReturn_EmptyCollection()
        {
            //Arrange
            var deviceController = new DeviceController(logger.Object, service.Object);
            string deviceId = "testDevice";
            var date = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
            var sensorType = string.Empty;
            service.Setup(s => s.GetDeviceDataAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new Dictionary<string, IEnumerable<CsvContent>>());

            //Act
            var response = await deviceController.GetDeviceDataAsync(deviceId, date);

            //Assert
            Assert.Empty(response);
        }
        [Fact]
        public async void SensorTypeInformed_ShouldReturn_DeviceData()
        {
            //Arrange
            var deviceController = new DeviceController(logger.Object, service.Object);
            string deviceId = "testDevice";
            var date = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
            var sensorType = "temperature";
            service.Setup(s => s.GetDeviceDataAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new Dictionary<string, IEnumerable<CsvContent>>()
                {
                    { "temperature" , new List<CsvContent>
                    {
                        new CsvContent
                        {
                            Date = DateTime.Now,
                            Info = 25.5f
                        }
                    } }
                });

            //Act
            var response = await deviceController.GetDeviceDataAsync(deviceId, date, sensorType);

            //Assert
            Assert.NotEmpty(response);
        }
    }
}
