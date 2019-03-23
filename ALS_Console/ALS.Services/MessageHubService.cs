using ALS.Services.Abstractions;
using Microsoft.Azure.EventHubs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ALS.Services.Utils.ConsoleLogger;
using static ALS.Services.Utils.TaskUtils;

namespace ALS.Services
{
    public class MessageHubService : IMessageService
    {
        // Event Hub-compatible endpoint
        // az iot hub show --query properties.eventHubEndpoints.events.endpoint --name {your IoT Hub name}
        private readonly string _eventHubsEndpoint = "sb://iothub-ns-als-iot-hu-1424708-15d76e184e.servicebus.windows.net/";

        // Event Hub-compatible name
        // az iot hub show --query properties.eventHubEndpoints.events.path --name {your IoT Hub name}
        private readonly string _eventHubsPath = "als-iot-hub";

        // az iot hub policy show --name iothubowner --query primaryKey --hub-name {your IoT Hub name}
        private readonly string _iotHubSasKey = "7vi3T4OghX/itOlnmgMu+iNXI9x0STitf7MQu1uOmxk=";
        private readonly string _iotHubSasKeyName = "iothubowner";
        
        private EventHubClient _eventHubClient;

        public MessageHubService()
        {
        }

        public bool Enable => true;

        public async Task Start(CancellationToken cancellationToken)
        {
            _eventHubClient = await ConnectClientAsync(cancellationToken).ConfigureAwait(false);
            if (_eventHubClient == null)
            {
                return;
            }

            var runtimeInfo = await _eventHubClient.GetRuntimeInformationAsync().ConfigureAwait(false);

            var tasks = new List<Task>();
            foreach (var partition in runtimeInfo.PartitionIds)
            {
                tasks.Add(ReceiveMessagesFromDeviceAsync(partition, cancellationToken));
            }

            Task.WaitAll(tasks.ToArray(), cancellationToken);
        }

        private async Task<EventHubClient> ConnectClientAsync(CancellationToken cancellationToken)
        {
            var connectionString = new EventHubsConnectionStringBuilder(new Uri(_eventHubsEndpoint), _eventHubsPath, _iotHubSasKeyName, _iotHubSasKey).ToString();

            var loop = true;
            var delay = TimeSpan.FromSeconds(5);

            while (loop)
            {
                LogInformation("Connecting with IoT Hub Event Hubs-compatible endpoint...");
                var eventHubClient = EventHubClient.CreateFromConnectionString(connectionString);
                if (eventHubClient != null)
                {
                    LogInformation($"Connected with success to '{eventHubClient.EventHubName}'.");
                    return eventHubClient;
                }

                LogError("Failed to connect event hub.");
                LogDebug($"Will retry after {delay.Seconds} seconds.");

                loop = await Wait(delay, cancellationToken).ConfigureAwait(false);
            }

            return null;
        }

        private async Task ReceiveMessagesFromDeviceAsync(string partition, CancellationToken cancellationToken)
        {
            var eventHubReceiver = _eventHubClient.CreateReceiver("$Default", partition, EventPosition.FromEnqueuedTime(DateTime.Now));

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                LogInformation($"Listening for messages on partition '{partition}'.");

                var events = await eventHubReceiver.ReceiveAsync(100, TimeSpan.FromSeconds(25));
                if (events == null)
                {
                    continue;
                }

                foreach (var eventData in events)
                {
                    string data = Encoding.UTF8.GetString(eventData.Body.Array);

                    var sb = new StringBuilder();

                    sb.AppendLine($"Message received on partition '{partition}'.");
                    sb.AppendLine($"\t{data}");        
                    
                    sb.AppendLine("\tApplication properties (set by device):");
                    foreach (var prop in eventData.Properties)
                    {
                        sb.AppendLine("\t\t{prop.Key}: {prop.Value}");
                    }

                    sb.AppendLine("\tSystem properties (set by IoT Hub):");
                    foreach (var prop in eventData.SystemProperties)
                    {
                        sb.AppendLine("\t\t{prop.Key}: {prop.Value}");
                    }

                    LogInformation(sb.ToString());
                }
            }
        }
    }
}
