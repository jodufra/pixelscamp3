using ALS.Services.Abstractions;
using ALS.Services.Utils;
using System;
using System.Threading;

namespace ALS.Services
{
    public static class ConsoleServiceHandler
    {
        public static void Run(string title, IMessageService service)
        {
            ConsoleUtils.WriteHeader(title);

            var cts = new CancellationTokenSource();

            var serviceTask = service.Start(cts.Token);

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
                Console.ReadKey();
            };

            serviceTask.Wait(cts.Token);
        }
    }
}
