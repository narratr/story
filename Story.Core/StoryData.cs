namespace Story.Core
{
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using Utils;

    [Serializable]
    public class StoryData : IStoryData
    {
        private readonly Dictionary<string, object> entries = new Dictionary<string, object>();

        public StoryData(IStory story)
        {
            Ensure.ArgumentNotNull(story, "story");
            this.Story = story;
        }

        [JsonIgnore]
        public bool IgnoreDuplicates { get; set; }

        protected IStory Story { get; private set; }

        public virtual object this[string key]
        {
            get
            {
                Ensure.ArgumentNotEmpty(key, "key");

                object result;
                return this.entries.TryGetValue(key, out result) ? result : null;
            }

            set
            {
                Ensure.ArgumentNotEmpty(key, "key");

                if (!IgnoreDuplicates && this.entries.ContainsKey(key))
                {
                    throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "Data already has key [{0}] with value", key));
                }

                this.entries[key] = value;
                this.Story.Log.Debug("Added key '{0}' to data.", key);
            }
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return this.entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.entries.GetEnumerator();
        }
    }
}
