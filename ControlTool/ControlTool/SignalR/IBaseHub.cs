using ControlTool.Models;
using Microsoft.AspNetCore.SignalR;

namespace ControlTool.SignalR
{
    public interface IBaseHub
    {
        /// <summary>
        /// Sending device reading
        /// </summary>
        /// <param name="deviceData">device data reading</param>
        /// <returns></returns>
        [HubMethodName("Reading")]
        Task PushDeviceReading(IMagicDevice deviceData);
    }
}
