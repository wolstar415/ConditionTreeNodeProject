public abstract class ConditionEvaluator<T>
{
    public abstract void Parse(string raw);
    public abstract bool Evaluate(T context);
    public abstract override string ToString();
}
