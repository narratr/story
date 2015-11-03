using System;
using System.Threading;
using System.Threading.Tasks;

namespace Story.Core.Utils
{
    /// <summary>
    /// Retrier
    /// </summary>
    public static class Retrier
    {
        /// <summary>
        /// Retries the specified asynchronous action
        /// </summary>
        /// <param name="asyncAction">The asynchronous action</param>
        /// <param name="tries">The number of tries</param>
        /// <param name="timeout">The timeout</param>
        public static async Task<T> RetryAsync<T>(Func<Task<T>> asyncAction, int tries = 3, int timeout = 100)
        {
            while (tries > 0)
            {
                tries--;

                try
                {
                    return await asyncAction();
                }
                catch
                {
                    if (tries <= 0)
                    {
                        throw;
                    }
                }

                await Task.Delay(timeout);
            }

            return default(T);
        }

        public static T Retry<T>(Func<T> action, int tries = 3, int timeout = 100)
        {
            while (tries > 0)
            {
                tries--;

                try
                {
                    return action();
                }
                catch
                {
                    if (tries <= 0)
                    {
                        throw;
                    }
                }

                Thread.Sleep(timeout);
            }

            return default(T);
        }
    }
}
