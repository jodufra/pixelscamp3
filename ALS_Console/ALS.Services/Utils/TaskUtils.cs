using System;
using System.Threading;
using System.Threading.Tasks;

namespace ALS.Services.Utils
{
    public static class TaskUtils
    {
        /// <summary>
        /// Creates an awaitable task that cancels itself after certain <paramref name="delay"/>.
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>true if the wait was not interrupted, false otherwise</returns>
        public static async Task<bool> Wait(TimeSpan delay, CancellationToken cancellationToken)
        {
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            return !cancellationToken.IsCancellationRequested;
        }
    }
}
