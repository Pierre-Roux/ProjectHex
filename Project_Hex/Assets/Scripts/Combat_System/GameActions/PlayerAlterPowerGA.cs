using System;
using System.Collections.Generic;

public class PlayerAlterPowerGA : GameAction
{
    public int Amount;
    public List<PermanentView> playerTargets;
    public List<EnemySlotView> enemyTargets;
    public bool passive;
    public PermaTypes permaTypes;
    public PlayerAlterPowerGA(int amount, bool Passive, PermaTypes PermaTypes, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets)
    {
        Amount = amount;
        passive = Passive;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
        permaTypes = PermaTypes;
    }
}
