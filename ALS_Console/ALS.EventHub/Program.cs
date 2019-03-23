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
            ConsoleServiceHandler.Run("ALS Event Hub", new MessageHubService());
        }
    }
}
