using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace ALS.Services.Utils
{
    public static class ConsoleUtils
    {
        public static void WriteHeader(string title)
        {
            var pad = Math.Max(0, 25 - title.Length / 2) + title.Length;
            WriteLine("########################################");
            WriteLine(title.PadLeft(pad));
            WriteLine("########################################\n");
            WriteLine("Ctrl-C to exit.\n");
        }

        public static bool PromptConfirmation(CancellationToken cancellationToken)
        {
            return PromptConfirmation("Do you want to continue?", cancellationToken);
        }

        public static bool PromptConfirmation(string question, CancellationToken cancellationToken)
        {
            var key = PromptKey(question, k => k == ConsoleKey.Escape || k == ConsoleKey.Enter, cancellationToken);
            return key == ConsoleKey.Enter;
        }

        public static ConsoleKey PromptAnyKey(CancellationToken cancellationToken)
        {
            return PromptKey("Press any key", null, cancellationToken);
        }

        public static ConsoleKey PromptAnyKey(string question, CancellationToken cancellationToken)
        {
            return PromptKey(question, null, cancellationToken);
        }

        public static ConsoleKey PromptKey(string question, Func<ConsoleKey, bool> predicate, CancellationToken cancellationToken)
        {
            if (predicate == null)
            {
                predicate = (ConsoleKey k) => true;
            }

            WriteLine(question);

            ConsoleKey key = default;
            do
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var keyInfo = ReadKey();
                key = keyInfo.Key;
            } while (!predicate(key));

            return key;
        }
    }
}
