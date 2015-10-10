namespace Story.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TFact"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface IRule<TFact, TResult>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fact"></param>
        /// <returns></returns>
        bool When(TFact fact);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fact"></param>
        /// <returns></returns>
        TResult Then(TFact fact);
    }
}
