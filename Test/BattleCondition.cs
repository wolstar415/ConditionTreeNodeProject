

using System;

public abstract class BattleCondition : ConditionEvaluator<BattleContext>
{
    public override abstract void Parse(string raw);
    public override abstract bool Evaluate(BattleContext context);
    public override abstract string ToString();
}

public class HPCondition : BattleCondition
{
    private Comparison _comparison;

    public override void Parse(string raw)
    {
        _comparison = Comparison.Parse(raw);
    }

    public override bool Evaluate(BattleContext context)
    {
        return _comparison.Check(context.HP);
    }

    public override string ToString() => $"HP:{_comparison}";
}

public class HasStateCondition : BattleCondition
{
    private string _state;
    public override void Parse(string raw) => _state = raw;
    public override bool Evaluate(BattleContext ctx) =>
        string.IsNullOrEmpty(_state)
            ? (ctx.States != null && ctx.States.Count > 0)
            : ctx.HasState(_state);
    public override string ToString() => string.IsNullOrEmpty(_state) ? "AnyState" : $"HasState({_state})";
}

public class IsStunCondition : BattleCondition
{
    public override void Parse(string raw) { }
    public override bool Evaluate(BattleContext ctx) => ctx.IsStun;
    public override string ToString() => "IsStun";
}

public class IsSilenceCondition : BattleCondition
{
    public override void Parse(string raw) { }
    public override bool Evaluate(BattleContext ctx) => ctx.IsSilence;
    public override string ToString() => "IsSilence";
}

public static class BattleConditionUtil
{

    public static ConditionTreeNode<BattleContext> LeafFactory(string raw)
    {
        int colon = raw.IndexOf(':');
        string typeStr = colon >= 0 ? raw.Substring(0, colon) : raw;
        string valueStr = colon >= 0 ? raw.Substring(colon + 1) : "";

        if (!Enum.TryParse(typeStr, out ConditionType type))
            throw new Exception("Unknown ConditionType: " + typeStr);

        ConditionEvaluator<BattleContext> cond = type switch
        {
            ConditionType.HP => new HPCondition(),
            ConditionType.HasState => new HasStateCondition(),
            ConditionType.IsStun => new IsStunCondition(),
            ConditionType.IsSilence => new IsSilenceCondition(),
            _ => throw new Exception("Not implemented type: " + type)
        };

        return new LeafNode<BattleContext>(cond, valueStr);
    }

}