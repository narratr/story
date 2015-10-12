namespace Story.Core
{
    /// <summary>
    /// A rule.
    /// </summary>
    /// <typeparam name="TFact">The type of the fact.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IRule<TFact, TResult>
    {
        /// <summary>
        /// When the specified fact is true
        /// </summary>
        /// <param name="fact">The fact.</param>
        /// <returns></returns>
        bool When(TFact fact);

        /// <summary>
        /// Then return the result
        /// </summary>
        /// <param name="fact">The fact.</param>
        /// <returns></returns>
        TResult Then(TFact fact);
    }
}
