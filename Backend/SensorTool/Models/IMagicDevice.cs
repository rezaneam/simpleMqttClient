namespace SensorTool.Models
{
    public interface IMagicDevice
    {
        bool Running { get; }
        int Id { get; }
        string Name { get; }
        double Speed { get; }
        double Battery { get; }
        double Temperature { get; }
        double Humidity { get; }
    }
}
