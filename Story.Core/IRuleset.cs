namespace Story.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IRuleset<TFact, TResult>
    {
        ICollection<IRule<TFact, TResult>> Rules { get; }

        IEnumerable<TResult> Fire(TFact fact);
    }

    public class Ruleset<TFact, TResult> : IRuleset<TFact, TResult>
    {
        public Ruleset()
        {
            this.Rules = new List<IRule<TFact, TResult>>();
        }

        public ICollection<IRule<TFact, TResult>> Rules { get; private set; }

        public IEnumerable<TResult> Fire(TFact fact)
        {
            return this.Rules.Where(rule => rule.When(fact)).Select(rule => rule.Then(fact));
        }
    }
}
