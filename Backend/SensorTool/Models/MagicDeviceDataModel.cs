namespace SensorTool.Models
{
    public record MagicDeviceDataModel(bool Running, int Id, string Name, double Speed, double Battery, double Temperature, double Humidity) : IMagicDevice;
}
