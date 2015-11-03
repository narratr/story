using System;
using System.Linq;
using System.Threading.Tasks;

namespace Story.Core
{
    public interface IStoryFactory
    {
        IStory CreateStory(string name);
    }

    public class BasicStoryFactory : IStoryFactory
    {
        private readonly IRuleset<IStory, IStoryHandler> ruleset;

        public BasicStoryFactory(IRuleset<IStory, IStoryHandler> ruleset)
        {
            this.ruleset = ruleset;
        }

        public IStory CreateStory(string name)
        {
            return new Story(name, this.ruleset);
        }
    }
}
