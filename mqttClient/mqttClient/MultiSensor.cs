using HiveMQtt.Client;
using HiveMQtt.Client.Events;
using HiveMQtt.Client.Options;
using System.Text.Json;

namespace mqttClient
{
    public class MultiSensor
    {
        private HiveMQClient? _mqttClient;
        private HiveMQClientOptions? _options;
        private string? _host;
        private int? _port;
        private string _topic = string.Empty;
        private readonly List<MagicDevice> _magicDevices;
        private readonly System.Timers.Timer _timer;

        public MultiSensor()
        {
            _magicDevices = new List<MagicDevice>();
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
            foreach (var device in _magicDevices)
                await _mqttClient.PublishAsync($"{_topic}/{device.Name}", JsonSerializer.Serialize(device), HiveMQtt.MQTT5.Types.QualityOfService.ExactlyOnceDelivery);
        }

        public void AddSensor(int sensorId)
        {
            if (_magicDevices.FirstOrDefault(item => item.Id == sensorId) != null) return;
            _magicDevices.Add(new MagicDevice(sensorId));
        }
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

        private void HandleConnected(object? sender, OnConnAckReceivedEventArgs e)
        {
            Console.WriteLine($"Connected to server {_host}:{_port} {e.ConnAckPacket.ReasonCode} {e.ConnAckPacket.AckFlags} {e.ConnAckPacket.ControlPacketType}"); ;
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
        }

    }
}
