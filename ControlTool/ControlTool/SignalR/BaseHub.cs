using ControlTool.Services;
using Microsoft.AspNetCore.SignalR;

namespace ControlTool.SignalR
{
    public class BaseHub : Hub<IBaseHub>
    {
        private readonly MqttService _mqttService;
        public BaseHub(MqttService mqttService)
        {
            _mqttService = mqttService;
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"New client [Id: {Context?.ConnectionId}] connected.");
            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"New client [Id: {Context?.ConnectionId}] disconnected.");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Invoked by SignalR Client request : Adding new sensor
        /// </summary>
        /// <param name="id">sensor Id</param>
        [HubMethodName("Add")]
        public void Add(int  id)
        {
            _mqttService?.AddSensor(id);
        }
    }
}
