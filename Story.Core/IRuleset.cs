namespace Story.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TFact"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface IRuleset<TFact, TResult>
    {
        /// <summary>
        /// 
        /// </summary>
        ICollection<IRule<TFact, TResult>> Rules { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fact"></param>
        /// <returns></returns>
        IEnumerable<TResult> Fire(TFact fact);
    }
}
