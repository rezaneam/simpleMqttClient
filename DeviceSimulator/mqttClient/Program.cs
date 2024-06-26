﻿namespace mqttClient
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Test Hive MQ app");
            var client = new MultiSensor();
            client.Connect("broker.hivemq.com", 8883, "CodeTestMqttTopic").Wait();

            client.AddSensor(10);
            client.Start(10);

            client.AddSensor(5);
            client.Start(5);

            client.AddSensor(20);

            Console.ReadLine();
        }
    }
}
