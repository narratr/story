namespace Story.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A set of rules.
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
        /// Invokes the set of rules with the given fact as parameter.
        /// </summary>
        /// <param name="fact">The fact.</param>
        /// <returns>A list of results from applicable rules.</returns>
        IEnumerable<TResult> Fire(TFact fact);
    }
}
