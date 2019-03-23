using ALS.Services.Abstractions;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ALS.Services.Utils.ConsoleLogger;
using static ALS.Services.Utils.TaskUtils;

namespace ALS.Services
{
    public class MessageClientService : IMessageService
    {
        private const int TemperatureThreshold = 30;

        private readonly string _connectionString = "HostName=ALS-Iot-Hub.azure-devices.net;DeviceId=Phone;SharedAccessKey=zW7xrws87s7xbsdikfiN1IaOZ+W2tdcfNlkcLBG6Gak=";
        private readonly TransportType _transportType = TransportType.Mqtt;

        private DeviceClient _deviceClient;
        private int _messageReceiveId;
        private int _messageSendId;

        public MessageClientService()
        {
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            _deviceClient = await ConnectClientAsync(cancellationToken);
            if (_deviceClient == null)
            {
                return;
            }

            var sendTask = SendEvents(cancellationToken);
            var receiveTask = ReceiveCommands(cancellationToken);

            Task.WaitAll(new[] { sendTask, receiveTask }, cancellationToken);
        }

        private async Task<DeviceClient> ConnectClientAsync(CancellationToken cancellationToken)
        {
            var delay = TimeSpan.FromSeconds(5);
            while (true)
            {
                LogInformation("Connecting client with IoT Hub...");
                var deviceClient = DeviceClient.CreateFromConnectionString(_connectionString, _transportType);
                if (deviceClient != null)
                {
                    LogInformation("Connected with success.");
                    LogDebug($"Device product info: '{deviceClient.ProductInfo}'");
                    return deviceClient;
                }

                LogError("Failed to connect device.");
                LogInformation($"Will retry after {delay.Seconds} seconds.");

                if (!await Wait(delay, cancellationToken))
                {
                    break;
                }
            }

            return null;
        }

        private async Task SendEvents(CancellationToken cancellationToken)
        {
            LogNewLine();
            LogInformation("Sending event messages to IoT Hub.");

            var random = new Random();
            var delay = TimeSpan.FromSeconds(5);

            while (true)
            {
                var temperature = random.Next(20, 35);
                var humidity = random.Next(60, 80);

                var data = new
                {
                    messageId = ++_messageSendId,
                    temperature,
                    humidity
                };

                var messageData = JsonConvert.SerializeObject(data);
                var eventMessage = new Message(Encoding.UTF8.GetBytes(messageData));
                eventMessage.Properties.Add("temperatureAlert", (temperature > TemperatureThreshold) ? "true" : "false");

                LogInformation($"Sending message: {_messageSendId}, Data: [{messageData}]");

                await _deviceClient.SendEventAsync(eventMessage, cancellationToken);

                if (!await Wait(delay, cancellationToken))
                {
                    break;
                }
            }
        }

        private async Task ReceiveCommands(CancellationToken cancellationToken)
        {
            LogNewLine();
            LogInformation("Receiving event messages to IoT Hub.");
            LogInformation("Use the IoT Hub Azure Portal to send a message to this device.");

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var receivedMessage = await _deviceClient.ReceiveAsync(cancellationToken);
                if (receivedMessage != null)
                {
                    var messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    LogInformation($"Received message: {++_messageReceiveId}, Data: [{messageData}]");

                    var propCount = 0;
                    foreach (var prop in receivedMessage.Properties)
                    {
                        LogInformation(string.Format("\t\tProperty[{0}] Key={1} : Value={2}", propCount++, prop.Key, prop.Value));
                    }

                    await _deviceClient.CompleteAsync(receivedMessage);
                }
            }
        }
    }
}
