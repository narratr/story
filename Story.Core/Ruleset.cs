namespace Story.Core
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TFact"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class Ruleset<TFact, TResult> : IRuleset<TFact, TResult>
    {
        /// <summary>
        /// 
        /// </summary>
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
