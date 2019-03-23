using ALS.Services;

namespace ALS.Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ConsoleServiceHandler.Run("ALS Client", new MessageClientService());
        }
    }
}
