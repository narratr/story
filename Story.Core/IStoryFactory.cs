using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Story.Core
{
    public interface IStoryFactory
    {
        IStory CreateStory(string name,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int? callerLineNumber = null);
    }

    public class BasicStoryFactory : IStoryFactory
    {
        private readonly IRuleset<IStory, IStoryHandler> ruleset;

        public BasicStoryFactory(IRuleset<IStory, IStoryHandler> ruleset)
        {
            this.ruleset = ruleset;
        }

        public IStory CreateStory(string name,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int? callerLineNumber = null)
        {
            return new Story(name, this.ruleset, false, callerMemberName, callerFilePath, callerLineNumber);
        }
    }
}
