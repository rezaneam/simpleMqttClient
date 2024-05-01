using HiveMQtt.Client.Options;
using HiveMQtt.Client;
using HiveMQtt.Client.Events;
using System.Text.Json;
using ControlTool.Models;
using HiveMQtt.MQTT5.Types;
using ControlTool.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace ControlTool.Services
{
    public class MqttService
    {
        const string CommandKeyWord = "Command";
        private HiveMQClient? _mqttClient;
        private HiveMQClientOptions? _options;
        private string? _host;
        private int? _port;
        private string _topic = string.Empty;
        private readonly SensorService _sensorService;
        private readonly IHubContext<BaseHub, IBaseHub> _hub;
        public MqttService(SensorService sensorService, IHubContext<BaseHub, IBaseHub> hub)
        {
            _sensorService = sensorService;
            _hub = hub;
        }

        public async void Connect(string host, int port, string topic)
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


        public async Task AddSensor(int sensorId)
        {
            if (_mqttClient == null) return;
            var publishResult = await _mqttClient.PublishAsync($"{_topic}/{CommandKeyWord}", JsonSerializer.Serialize(new MqttCommand("Add", sensorId)));
        }

        public async Task StartSensor(int sensorId)
        {
            if (_mqttClient == null) return;
            var publishResult = await _mqttClient.PublishAsync($"{_topic}/{CommandKeyWord}", JsonSerializer.Serialize(new MqttCommand("Start", sensorId)));
        }


        private async Task Subsribe()
        {
            if (_mqttClient == null) return;
            var builder = new SubscribeOptionsBuilder();
            builder.WithSubscription($"{_topic}/#", QualityOfService.ExactlyOnceDelivery);
                 
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

            if (!int.TryParse(topic.Replace($"{_topic}/MagicDevice-", ""), out var id)) return;

            var sensorReading = JsonSerializer.Deserialize<MagicDeviceDataModel>(message);

            if (sensorReading == null) return;

            _sensorService.AddSensorReading(sensorReading);
            _hub.Clients.All.PushDeviceReading(sensorReading);

        }

    }
}
