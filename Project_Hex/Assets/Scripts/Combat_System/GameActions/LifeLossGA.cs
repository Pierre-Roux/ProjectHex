using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeLossGA : GameAction
{
    public int Amount { get; set; }
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }

    public LifeLossGA(int amount, List<PermanentView> targets_Player = null, List<EnemySlotView> targets_Enemy = null)
    {
        Amount = amount;
        playerTargets = targets_Player;
        enemyTargets = targets_Enemy;
    }
}
