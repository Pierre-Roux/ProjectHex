using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeLossGA : GameAction
{
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }
    public int Amount;
    public PlayerLifeLossGA(int amount, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets)
    {
        Amount = amount;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
    }
}
