using SensorTool.Models;
using SensorTool.Services;
using Microsoft.AspNetCore.Mvc;

namespace SensorTool.Controllers
{
    [ApiController]
    public class BasicController : ControllerBase
    {
        private readonly SensorService _sensorService;
        private readonly MqttService  _mqttService;

        public BasicController(SensorService sensorService, MqttService mqttService)
        {
            _sensorService = sensorService;
            _mqttService = mqttService;
        }

        [HttpGet("Hello")]
        public string HelloWolrd()
        {
            return "Hello World";
        }

        [HttpGet("Get")]
        public IEnumerable<IMagicDevice> Get()
        {
            try
            {
                return _sensorService.GetDevices();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to fetch list of Devices. Details: {e.Message}");
                throw;
            }
        }


        [HttpPost("Add")]
        public void AddDevice([FromBody] int sensorId)
        {

            try
            {
                _mqttService?.AddSensor(sensorId);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to add Device {sensorId}. Details: {e.Message}");
                throw;
            }

        }
    }
}
