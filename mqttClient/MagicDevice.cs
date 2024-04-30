namespace mqttClient
{
    public class MagicDevice
    {
        public bool Running { get; private set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public double Speed { get; set; }
        public double Battery { get; private set; }
        public double Temperature { get; private set; }
        public double Humidity { get; private set; }

        public MagicDevice(int Id) {
            Name = $"MagicDevice-{Id}";
            Speed = new Random().NextDouble() * 30;
            Battery = 100;
            Temperature = 25 + GetRangeValue(-2, 2);
            Humidity = 50 + GetRangeValue(-25, 25);
        }

        public MagicDevice Read()
        {
            if (!Running) return this;

            Battery -= new Random().NextDouble();
            Speed += GetRangeValue(-1, 1);
            Temperature += GetRangeValue(-0.1, 0.5);
            Humidity += GetRangeValue(-0.2, 0.2);

            if(Battery <0)
            {
                Battery = 0;
                Running = false;
            }
            return this;
        }

        public void Start() => Running = true;
        public void Stop() => Running = false;

        private double GetRangeValue(double min, double max) => ((max - min) * new Random().NextDouble()) + min;
    }
}
