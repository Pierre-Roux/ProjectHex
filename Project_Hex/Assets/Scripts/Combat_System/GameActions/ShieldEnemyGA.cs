using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEnemyGA : GameAction
{
    public List<PermanentView> playerTargets;
    public List<EnemySlotView> enemyTargets;

    public ShieldEnemyGA(List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets)
    {
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
    }
}
