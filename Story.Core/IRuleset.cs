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
}
