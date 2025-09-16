using System;
using System.Collections.Generic;

public class AttackEnemyGA : GameAction
{
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }
    public int Damage;
    public AttackEnemyGA(int damage, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets)
    {
        Damage = damage;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
    }
}
