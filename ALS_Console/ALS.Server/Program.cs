using ALS.Services;

namespace ALS.Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ConsoleServiceHandler.Run("ALS Client", new MessageServerService());
        }
    }
}
