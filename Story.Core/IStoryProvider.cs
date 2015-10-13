namespace Story.Core
{
    public interface IStoryProvider
    {
        IStoryRulesetProvider StoryRulesetProvider { get; }

        IStory CreateStory(string name);
    }

    public class BasicStoryProvider : IStoryProvider
    {
        public BasicStoryProvider(IStoryRulesetProvider storyRulesetProvider)
        {
            StoryRulesetProvider = storyRulesetProvider;
        }

        public IStoryRulesetProvider StoryRulesetProvider { get; set; }

        public IStory CreateStory(string name)
        {
            return new Story(name, StoryRulesetProvider.GetRuleset());
        }
    }
}
