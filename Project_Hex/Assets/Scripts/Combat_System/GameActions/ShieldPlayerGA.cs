using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPlayerGA : GameAction
{
    public List<PermanentView> playerTargets;
    public List<EnemySlotView> enemyTargets;

    public ShieldPlayerGA(List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets)
    {
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
    }
}
