using HiveMQtt.Client;
using HiveMQtt.Client.Events;
using HiveMQtt.Client.Options;
using HiveMQtt.MQTT5.Types;
using System.Collections.Concurrent;
using System.Text.Json;

namespace mqttClient
{
    public class MultiSensor
    {
        const string CommandKeyWord = "Command";
        private HiveMQClient? _mqttClient;
        private HiveMQClientOptions? _options;
        private string? _host;
        private int? _port;
        private string _topic = string.Empty;
        private readonly ConcurrentDictionary<int, MagicDevice> _magicDevices;
        private readonly System.Timers.Timer _timer;

        public MultiSensor()
        {
            _magicDevices = [];
            _timer = new System.Timers.Timer(1000)
            {
                AutoReset = true,
                Enabled = true
            };

            _timer.Start();
            _timer.Elapsed += UpdateValues;
        }

        private async void UpdateValues(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (_mqttClient == null) return;
            foreach (var device in _magicDevices.Values)
                await _mqttClient.PublishAsync($"{_topic}/{device.Name}", JsonSerializer.Serialize(device.Read()), QualityOfService.ExactlyOnceDelivery); 
        }

        public async void AddSensor(int sensorId)
        {
            if (_magicDevices.ContainsKey(sensorId)) return;
            var sensor = new MagicDevice(sensorId);
            _magicDevices.TryAdd(sensorId, sensor);
            if (_mqttClient == null) return;
            await _mqttClient.PublishAsync($"{_topic}/{sensor.Name}", JsonSerializer.Serialize(sensor.Read()), QualityOfService.ExactlyOnceDelivery);
        }

        public void RemoveSensor(int sensorId)
        {
            if (!_magicDevices.ContainsKey(sensorId)) return;
           
            _magicDevices.TryRemove(sensorId, out var _);
        }

        public void Start(int sensorId)
        {
            if (!_magicDevices.ContainsKey(sensorId)) return;
            _magicDevices[sensorId].Start() ;

        }

        public void Stop(int sensorId)
        {
            if (!_magicDevices.ContainsKey(sensorId)) return;
            _magicDevices[sensorId].Stop();
        }

        #region IgnoreMe
        public async Task Connect(string host, int port, string topic)
        {
            _host = host;
            _port = port;
            _topic = topic; 
            _options = new HiveMQClientOptionsBuilder().
                    WithBroker(host).
                    WithPort(port).
                    WithUseTls(true).
                    Build();
            _mqttClient = new HiveMQClient(_options);

            _mqttClient.OnMessageReceived += HandleReceivceMessages;
            _mqttClient.OnDisconnectReceived += HandleDiconnect;
            _mqttClient.OnConnectSent += HandleConnecting;
            _mqttClient.OnConnAckReceived += HandleConnected;

            var connectResult = await _mqttClient.ConnectAsync().ConfigureAwait(false);
        }

        private async Task Subsribe()
        {
            if (_mqttClient == null) return;
            var builder = new SubscribeOptionsBuilder();
            builder.WithSubscription($"{_topic}/{CommandKeyWord}", QualityOfService.ExactlyOnceDelivery);

            var subscribeOptions = builder.Build();
            var subscribeResult = await _mqttClient.SubscribeAsync(subscribeOptions);
        }

        private async void HandleConnected(object? sender, OnConnAckReceivedEventArgs e)
        {
            Console.WriteLine($"Connected to server {_host}:{_port} {e.ConnAckPacket.ReasonCode} {e.ConnAckPacket.AckFlags} {e.ConnAckPacket.ControlPacketType}");
            await Subsribe();
        }

        private void HandleConnecting(object? sender, OnConnectSentEventArgs e)
        {
            Console.WriteLine($"Connecting to server {_host}:{_port} {e.ConnectPacket.ControlPacketType}");
        }

        private void HandleDiconnect(object? sender, OnDisconnectReceivedEventArgs e)
        {
            Console.WriteLine($"Disconnected from {_host}:{_port}. Reason: {e.DisconnectPacket.DisconnectReasonCode}. ControlPacketType: {e.DisconnectPacket.ControlPacketType}");
        }

        private void HandleReceivceMessages(object? sender, OnMessageReceivedEventArgs arg)
        {
            var message = arg.PublishMessage.PayloadAsString;
            var topic = arg.PublishMessage.Topic;
            if (string.IsNullOrEmpty(_topic) || string.IsNullOrEmpty(topic) || message == null || !topic.Contains(_topic))
                return;

            if(!topic.Contains(CommandKeyWord, StringComparison.InvariantCultureIgnoreCase))
                return;

            try
            {
                var command = JsonSerializer.Deserialize<MqttCommand>(message);
                if (command == null) return;

                switch (command.Command.ToLower()) 
                {
                    case "add":
                        AddSensor(command.SensorId);
                        break;

                    case "remove":
                        RemoveSensor(command.SensorId);
                        break;

                    case "start":
                        Start(command.SensorId); 
                        break;

                    case "stop":
                        Stop(command.SensorId); 
                        break;

                    default:
                        Console.WriteLine($"Command {command.Command} is not supported");
                        break;
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Failed to process the received message {message}. Details {ex}");
            }
        }

        #endregion

    }
}
