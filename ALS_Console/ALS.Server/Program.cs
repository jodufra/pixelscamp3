using System;
using ALS.Services;
using ALS.Services.Utils;
using System;
using System.Diagnostics;
using System.Threading;

namespace ALS.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleUtils.WriteHeader("ALS Client");

            var cts = new CancellationTokenSource();

            var client = new MessageServerService();
            var clientTask = client.Start(cts.Token);

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
                Console.ReadKey();
            };

            try
            {
                clientTask.Wait(cts.Token);
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
