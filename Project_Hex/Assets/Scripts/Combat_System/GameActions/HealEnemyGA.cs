using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEnemyGA : GameAction
{
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }
    public int HealAmount;

    public HealEnemyGA(int healAmount, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets)
    {
        HealAmount = healAmount;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
    }
}
