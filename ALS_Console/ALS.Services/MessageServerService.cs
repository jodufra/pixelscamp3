using ALS.Services.Abstractions;
using ALS.Services.Utils;
using Microsoft.Azure.Devices;
using System;
using System.Threading;
using System.Threading.Tasks;
using static ALS.Services.Utils.ConsoleLogger;
using static ALS.Services.Utils.TaskUtils;

namespace ALS.Services
{
    public class MessageServerService : IMessageService
    {
        private const int TemperatureThreshold = 30;

        private readonly string _connectionString = "HostName=ALS-Iot-Hub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=7vi3T4OghX/itOlnmgMu+iNXI9x0STitf7MQu1uOmxk=";

        private ServiceClient _serviceClient;

        public MessageServerService()
        {
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            _serviceClient = await ConnectClientAsync(cancellationToken);
            if (_serviceClient == null)
            {
                return;
            }

            var sendTask = SendCommands(cancellationToken);
            sendTask.Wait(cancellationToken);
        }

        private async Task<ServiceClient> ConnectClientAsync(CancellationToken cancellationToken)
        {
            var delay = TimeSpan.FromSeconds(5);
            while (true)
            {
                LogInformation("Connecting service with IoT Hub...");
                var serviceClient = ServiceClient.CreateFromConnectionString(_connectionString);
                if (serviceClient != null)
                {
                    LogInformation("Connected with success.");
                    return serviceClient;
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

        private async Task SendCommands(CancellationToken cancellationToken)
        {
            LogNewLine();
            LogInformation("Sending commands to IoT Hub.");

            var timeout = TimeSpan.FromSeconds(30);

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                ConsoleUtils.PromptAnyKey(cancellationToken);

                var methodInvocation = new CloudToDeviceMethod("LedColor")
                {
                    ResponseTimeout = timeout
                };

                var response = await _serviceClient.InvokeDeviceMethodAsync("Default", methodInvocation);
                LogInformation($"Response status: {response.Status}.");
            }
        }
    }
}
