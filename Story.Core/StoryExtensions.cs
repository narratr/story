namespace Story.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class StoryExtensions
    {
        /// <summary>
        /// Invokes the task to be observed by this story.
        /// </summary>
        /// <typeparam name="T">The task result type.</typeparam>
        /// <param name="func">Function returning a task to observe.</param>
        /// <returns>The result.</returns>
        public static T Run<T>(this IStory story, Func<IStory, T> func)
        {
            var tcs = new TaskCompletionSource<T>();

            try
            {
                story.Start();

                var result = func(story);
                tcs.TrySetResult(result);

                return result;
            }
            catch (Exception exception)
            {
                tcs.TrySetException(exception);
                throw;
            }
            finally
            {
                story.Stop(tcs.Task);
            }
        }

        /// <summary>
        /// Invokes the task to be observed by this story.
        /// </summary>
        /// <param name="action">Action to observe.</param>
        public static void Run(this IStory story, Action<IStory> action)
        {
            var tcs = new TaskCompletionSource<object>();

            try
            {
                story.Start();
                action(story);
                tcs.TrySetResult(null);
            }
            catch (Exception exception)
            {
                tcs.TrySetException(exception);
                throw;
            }
            finally
            {
                story.Stop(tcs.Task);
            }
        }

        /// <summary>
        /// Invokes the task to be observed by this story.
        /// </summary>
        /// <typeparam name="T">The task result type.</typeparam>
        /// <param name="func">Function returning a task to observe.</param>
        /// <returns>The task observed by this story.</returns>
        public static Task<T> RunAsync<T>(this IStory story, Func<IStory, Task<T>> func)
        {
            story.Start();

            Task<T> result = func(story);
            result.ContinueWith(story.Stop, TaskContinuationOptions.ExecuteSynchronously);

            return result;
        }

        /// <summary>
        /// Invokes the task to be observed by this story.
        /// </summary>
        /// <param name="func">Function returning a task to observe.</param>
        /// <returns>The task observed by this story.</returns>
        public static Task RunAsync(this IStory story, Func<IStory, Task> func)
        {
            story.Start();

            Task result = func(story);
            result.ContinueWith(story.Stop, TaskContinuationOptions.ExecuteSynchronously);

            return result;
        }
    }
}
