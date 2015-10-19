using System.Runtime.CompilerServices;

namespace Story.Core
{
    public interface IStoryProvider
    {
        IStoryRulesetProvider StoryRulesetProvider { get; }

        IStory CreateStory(string name);
    }

    public static class StoryProviderExtensions
    {
        public static IStory CreateStory(this IStoryProvider storyProvider, [CallerMemberName]string name = "")
        {
            return storyProvider.CreateStory(name);
        }
    }

    public class BasicStoryProvider : IStoryProvider
    {
        public BasicStoryProvider(IStoryRulesetProvider storyRulesetProvider)
        {
            StoryRulesetProvider = storyRulesetProvider;
        }

        public IStoryRulesetProvider StoryRulesetProvider { get; set; }

        public virtual IStory CreateStory(string name)
        {
            return new Story(name, StoryRulesetProvider.GetRuleset());
        }
    }
}
