namespace Story.Core
{
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
            IgnoreDuplicates = true;
            this.Story = story;
        }

        public bool IgnoreDuplicates { get; set; }

        protected IStory Story { get; private set; }

        public virtual object this[string key]
        {
            get
            {
                if (key == null) return null; // Ignore null keys

                object result;
                return this.entries.TryGetValue(key, out result) ? result : null;
            }

            set
            {
                if (key == null) return; // Ignore null keys

                if (!IgnoreDuplicates && this.entries.ContainsKey(key))
                {
                    throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "Data already has key [{0}] with value", key));
                }

                this.entries[key] = value;
                // this.Story.Log.Debug("Added key '{0}' to data.", key);
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
