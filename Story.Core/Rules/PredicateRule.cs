namespace Story.Core.Rules
{
    using System;
    using System.Linq;

    public class PredicateRule : IRule<IStory, IStoryHandler>
    {
        private readonly Func<IStory, bool> predicate;
        private readonly Func<IStory, IStoryHandler> valueFactory;

        public PredicateRule(Func<IStory, bool> predicate, Func<IStory, IStoryHandler> valueFactory)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }

            this.predicate = predicate;
            this.valueFactory = valueFactory;
        }

        public bool When(IStory story)
        {
            return this.predicate(story);
        }

        public IStoryHandler Then(IStory story)
        {
            return this.valueFactory(story);
        }
    }
}
