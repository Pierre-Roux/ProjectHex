using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeLossGA : GameAction
{
    public List<PermanentView> playerTargets;
    public List<EnemySlotView> enemyTargets;
    public int Amount;
    public PlayerLifeLossGA(int amount, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets)
    {
        Amount = amount;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
    }
}
