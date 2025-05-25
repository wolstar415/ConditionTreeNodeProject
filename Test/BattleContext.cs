using System.Collections.Generic;

public class BattleContext
{
    public int HP;
    public HashSet<string> States;
    public bool IsStun;
    public bool IsSilence;

    public bool HasState(string state) => States != null && States.Contains(state);
}

public enum ConditionType
{
    HP,
    HasState,
    IsStun,
    IsSilence,
}
