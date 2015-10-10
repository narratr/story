namespace Story.Core
{
    public interface IRule<TFact, TResult>
    {
        bool When(TFact fact);

        TResult Then(TFact fact);
    }
}
