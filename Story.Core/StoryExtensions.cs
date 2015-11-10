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
        /// <param name="storyFactory">The story factory.</param>
        /// <param name="name">The name.</param>
        /// <param name="func">Function returning a task to observe.</param>
        /// <returns>
        /// The result.
        /// </returns>
        public static T StartNew<T>(this IStoryFactory storyFactory, string name, Func<T> func)
        {
            Ensure.ArgumentNotNull(storyFactory, "storyFactory");
            Ensure.ArgumentNotNull(func, "func");

            IStory story = storyFactory.CreateStory(name);

            try
            {
                story.Start();

                return func();
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
        /// <param name="storyFactory">The story factory.</param>
        /// <param name="name">The name.</param>
        /// <param name="action">Action to observe.</param>
        public static void StartNew(this IStoryFactory storyFactory, string name, Action action)
        {
            Ensure.ArgumentNotNull(storyFactory, "storyFactory");
            Ensure.ArgumentNotNull(action, "action");

            IStory story = storyFactory.CreateStory(name);

            try
            {
                story.Start();

                action();
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
        /// <param name="storyFactory">The story factory.</param>
        /// <param name="name">The name.</param>
        /// <param name="func">Function returning a task to observe.</param>
        /// <returns>
        /// The task observed by this story.
        /// </returns>
        public static Task<T> StartNewAsync<T>(this IStoryFactory storyFactory, string name, Func<Task<T>> func)
        {
            Ensure.ArgumentNotNull(storyFactory, "storyFactory");
            Ensure.ArgumentNotNull(func, "func");

            IStory story = storyFactory.CreateStory(name);

            story.Start();

            Task<T> result = func();
            result.ContinueWith(story.Stop, TaskContinuationOptions.ExecuteSynchronously);

            return result;
        }

        /// <summary>
        /// Invokes the task to be observed by this story.
        /// </summary>
        /// <param name="storyFactory">The story factory.</param>
        /// <param name="name">The name.</param>
        /// <param name="func">Function returning a task to observe.</param>
        /// <returns>
        /// The task observed by this story.
        /// </returns>
        public static Task StartNewAsync(this IStoryFactory storyFactory, string name, Func<Task> func)
        {
            Ensure.ArgumentNotNull(storyFactory, "storyFactory");
            Ensure.ArgumentNotNull(func, "func");

            IStory story = storyFactory.CreateStory(name);

            story.Start();

            Task result = func();
            result.ContinueWith(story.Stop, TaskContinuationOptions.ExecuteSynchronously);

            return result;
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
        /// Gets data values
        /// </summary>
        /// <param name="story">The story.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        public static IEnumerable<KeyValuePair<string, object>> GetDataValues(this IStory story, bool recursive = true)
        {
            Ensure.ArgumentNotNull(story, "story");

            foreach (var data in story.Data)
            {
                yield return data;
            }

            if (recursive && story.Children != null)
            {
                foreach (var childStory in story.Children)
                {
                    foreach (var data in childStory.GetDataValues(recursive))
                    {
                        yield return data;
                    }
                }
            }
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
        public static IStoryData GetData(this IStory story, bool recursive = true)
        {
            Ensure.ArgumentNotNull(story, "story");

            if (recursive == false)
            {
                return story.Data;
            }

            IStoryData storyData = new StoryData(story)
            {
                IgnoreDuplicates = true
            };
            AddData(storyData, story);

            return storyData;
        }

        private static void AddData(IStoryData storyData, IStory story)
        {
            foreach (var data in story.Data)
            {
                storyData[data.Key] = data.Value;
            }

            if (story.Children != null)
            {
                foreach (var childStory in story.Children)
                {
                    AddData(storyData, childStory);
                }
            }
        }

        /// <summary>
        /// Gets the logs from story and child stories.
        /// </summary>
        public static IEnumerable<IStoryLogEntry> GetLogs(this IStory story, bool recursive = true)
        {
            Ensure.ArgumentNotNull(story, "story");

            foreach (var log in story.Log)
            {
                yield return log;
            }

            if (recursive && story.Children != null)
            {
                foreach (var childStory in story.Children)
                {
                    foreach (var log in childStory.GetLogs(recursive))
                    {
                        yield return log;
                    }
                }
            }
        }

        private static IEnumerable<T> Flatten<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> childrenSelector)
        {
            Ensure.ArgumentNotNull(source, "source");
            Ensure.ArgumentNotNull(childrenSelector, "childrenSelector");

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
