using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEnemyGA : GameAction
{
    public List<PermanentView> playerTargets;
    public List<EnemySlotView> enemyTargets;
    public int HealAmount;

    public HealEnemyGA(int healAmount, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets)
    {
        HealAmount = healAmount;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
    }
}
