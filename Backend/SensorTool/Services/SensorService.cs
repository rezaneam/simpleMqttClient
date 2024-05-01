using SensorTool.Models;
using System.Collections.Concurrent;

namespace SensorTool.Services
{
    public class SensorService
    {
        private readonly ConcurrentDictionary<int, MagicDeviceDataModel> _magicDevices;

        public SensorService()
        {
            _magicDevices = [];
        }

        public void AddSensorReading(MagicDeviceDataModel sensorData)
        {
            _magicDevices[sensorData.Id] = sensorData;
        }

        public IList<IMagicDevice> GetDevices()
        {
            return _magicDevices.Values.Cast<IMagicDevice>().ToList();
        }

        public IMagicDevice GetDevice(int id)
        {
            return _magicDevices[id];
        }

    }
}
