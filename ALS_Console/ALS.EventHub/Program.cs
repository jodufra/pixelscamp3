using ALS.Services;
using ALS.Services.Utils;
using System;
using System.Diagnostics;
using System.Threading;

namespace ALS.EventHub
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ConsoleUtils.WriteHeader("ALS Event Hub");

            var cts = new CancellationTokenSource();

            var hub = new MessageHubService();
            var hubTask = hub.Start(cts.Token);

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
                Console.ReadKey();
            };

            try
            {
                hubTask.Wait(cts.Token);
            }
            catch (OperationCanceledException)
            {
                // silent
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw e;
            }
        }
    }
}
