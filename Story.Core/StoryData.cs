namespace Story.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Utils;

    [Serializable]
    public class StoryData : IStoryData
    {
        private readonly Dictionary<string, object> entries = new Dictionary<string, object>();
        private readonly IStory story;

        public StoryData(IStory story)
        {
			Ensure.ArgumentNotNull(story, "story");
            this.story = story;
        }

        public object this[string key]
        {
            set
            {
				Ensure.ArgumentNotEmpty(key, "key");

                this.entries[key] = value;
                this.story.Log.Debug("Added key '{0}' to data.", key);
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
