using System;
using System.Collections.Generic;

public class AttackPlayerGA : GameAction
{
    public List<PermanentView> playerTargets;
    public List<EnemySlotView> enemyTargets;
    public int Damage;
    public AttackPlayerGA(int damage, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets)
    {
        Damage = damage;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
    }
}
