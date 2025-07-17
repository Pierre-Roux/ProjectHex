using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldGA : GameAction
{
    public int Amount { get; set; }
    public List<PermanentView> Targets_Player { get; set; }
    public List<EnemySlotView> Targets_Enemy { get; set; }

    public ShieldGA(int amount, List<PermanentView> targets_Player = null, List<EnemySlotView> targets_Enemy = null)
    {
        Amount = amount;
        Targets_Player = targets_Player;
        Targets_Enemy = targets_Enemy;
    }
}
