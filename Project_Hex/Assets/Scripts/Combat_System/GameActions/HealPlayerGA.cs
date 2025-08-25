using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPlayerGA : GameAction
{
    public List<PermanentView> playerTargets;
    public List<EnemySlotView> enemyTargets;
    public int HealAmount;

    public HealPlayerGA(int healAmount, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets)
    {
        HealAmount = healAmount;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
    }
}
