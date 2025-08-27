using System;
using System.Collections.Generic;

public class PlayerAlterPowerGA : GameAction
{
    public int Amount;
    public List<PermanentView> playerTargets;
    public List<EnemySlotView> enemyTargets;
    public TargetMode targetMode;
    public bool passive;
    public PermaTypes permaTypes;
    public PlayerAlterPowerGA(int amount, bool Passive, PermaTypes PermaTypes, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets, TargetMode TargetMode = TargetMode.Self)
    {
        Amount = amount;
        passive = Passive;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
        permaTypes = PermaTypes;
        targetMode = TargetMode;
    }
}
