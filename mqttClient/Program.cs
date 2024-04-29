namespace mqttClient
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Test Hive MQ app");
            var client = new MultiSensor();
            client.Connect("broker.hivemq.com", 8883, "Gjermund").Wait();

            client.AddSensor(10);
            Console.ReadLine();
        }
    }
}
