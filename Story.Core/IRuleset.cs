namespace Story.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TFact">The type of the fact.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IRuleset<TFact, TResult>
    {
        /// <summary>
        /// Gets the rules.
        /// </summary>
        ICollection<IRule<TFact, TResult>> Rules { get; }

        /// <summary>
        /// Fires the specified fact.
        /// </summary>
        /// <param name="fact">The fact.</param>
        /// <returns></returns>
        IEnumerable<TResult> Fire(TFact fact);
    }
}
