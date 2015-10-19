namespace Story.Core
{
    using global::Story.Core.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            Ensure.ArgumentNotNull(story, "story");
            Ensure.ArgumentNotNull(func, "func");

            var tcs = new TaskCompletionSource<T>();

            try
            {
                story.Start();

                var result = func(story);
                tcs.SetResult(result);

                return result;
            }
            catch (Exception exception)
            {
                tcs.SetException(exception);
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
            Ensure.ArgumentNotNull(story, "story");
            Ensure.ArgumentNotNull(action, "action");

            try
            {
                story.Start();

                action(story);
            }
            catch (Exception exception)
            {
                story.Data["exception"] = exception;
                throw;
            }
            finally
            {
                story.Stop();
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
            Ensure.ArgumentNotNull(story, "story");
            Ensure.ArgumentNotNull(func, "func");

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
            Ensure.ArgumentNotNull(story, "story");
            Ensure.ArgumentNotNull(func, "func");

            story.Start();

            Task result = func(story);
            result.ContinueWith(story.Stop, TaskContinuationOptions.ExecuteSynchronously);

            return result;
        }

        /// <summary>
        /// Write to the log of the story with debug log severity
        /// </summary>
        public static void Debug(this IStory story, string format, params object[] args)
        {
            Ensure.ArgumentNotNull(story, "story");

            story.Log.Debug(format, args);
        }

        /// <summary>
        /// Write to the log of the story with information log severity
        /// </summary>
        public static void Info(this IStory story, string format, params object[] args)
        {
            Ensure.ArgumentNotNull(story, "story");

            story.Log.Info(format, args);
        }

        /// <summary>
        /// Write to the log of the story with warning log severity
        /// </summary>
        public static void Warn(this IStory story, string format, params object[] args)
        {
            Ensure.ArgumentNotNull(story, "story");

            story.Log.Warn(format, args);
        }

        /// <summary>
        /// Write to the log of the story with error log severity
        /// </summary>
        public static void Error(this IStory story, string format, params object[] args)
        {
            Ensure.ArgumentNotNull(story, "story");

            story.Log.Error(format, args);
        }

        /// <summary>
        /// Write to the log of the story with critical log severity
        /// </summary>
        public static void Critical(this IStory story, string format, params object[] args)
        {
            Ensure.ArgumentNotNull(story, "story");

            story.Log.Critical(format, args);
        }

        /// <summary>
        /// Get the first value from the story data in the story
        /// and the children stories inside (recursive)
        /// or null if not found
        /// </summary>
        public static object GetDataValue(this IStory story, string key, bool recursive = true)
        {
            Ensure.ArgumentNotNull(story, "story");

            // assume story.Data[key] returns null if not found
            object result = story.Data[key];
            if (result != null || recursive == false)
            {
                return result;
            }

            if (story.Children != null)
            {
                foreach (var childStory in story.Children)
                {
                    result = childStory.GetDataValue(key, recursive);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get all values with the responding story from the story data in the story
        /// and from the children stories inside
        /// or an empty enumerable if not found
        /// </summary>
        public static IEnumerable<StoryDataValue> GetDataValues(this IStory story, string key)
        {
            Ensure.ArgumentNotNull(story, "story");

            List<StoryDataValue> result = null;

            // assume story.Data[key] returns null if not found
            object value = story.Data[key];

            if (value != null)
            {
                result = new List<StoryDataValue>();
                result.Add(new StoryDataValue(story, value));
            }

            if (story.Children != null)
            {
                foreach (var childStory in story.Children)
                {
                    var childResults = GetDataValues(childStory, key);
                    if (childResults.Any())
                    {
                        if (result == null)
                        {
                            result = new List<StoryDataValue>();
                        }

                        result.AddRange(childResults);
                    }
                }
            }

            return result ?? Enumerable.Empty<StoryDataValue>();
        }

        /// <summary>
        /// Determines whether the story is root.
        /// </summary>
        public static bool IsRoot(this IStory story)
        {
            return story != null && story.Parent == null;
        }

        /// <summary>
        /// Gets the data from story and child stories.
        /// </summary>
        public static IEnumerable<KeyValuePair<string, object>> GetData(this IStory story, bool recursive = true)
        {
            Ensure.ArgumentNotNull(story, "story");

            if (recursive == false)
            {
                return story.Data;
            }

            return story.Data.Union(story.Children.Flatten(childStory => childStory.Children).SelectMany(childStory => childStory.Data));
        }

        /// <summary>
        /// Gets the logs from story and child stories.
        /// </summary>
        public static IEnumerable<IStoryLogEntry> GetLogs(this IStory story, bool recursive = true)
        {
            Ensure.ArgumentNotNull(story, "story");

            if (recursive == false)
            {
                return story.Log;
            }

            return story.Log.Union(story.Children.Flatten(childStory => childStory.Children).SelectMany(childStory => childStory.Log));
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> childrenSelector)
        {
            LinkedList<T> list = new LinkedList<T>(source);

            while (list.Count > 0)
            {
                var item = list.First.Value;

                yield return item;

                list.RemoveFirst();

                var node = list.First;
                foreach (var child in childrenSelector(item))
                {
                    if (node != null)
                    {
                        list.AddBefore(node, child);
                    }
                    else
                    {
                        list.AddLast(child);
                    }
                }
            }
        }

        private static void Stop(this IStory story, Task task)
        {
            Ensure.ArgumentNotNull(story, "story");
            Ensure.ArgumentNotNull(task, "task");

            story.Task = task;
            story.Stop();
        }
    }

    public class StoryDataValue
    {
        public StoryDataValue(IStory story, object value)
        {
            Story = story;
            Value = value;
        }

        public IStory Story { get; private set; }

        public object Value { get; private set; }
    }
}
