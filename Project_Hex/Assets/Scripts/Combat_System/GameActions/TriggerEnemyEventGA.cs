using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnemyEventGA : GameAction
{
    public Events gameEvent;
    public EnemySlotView enemySlotView;

    public TriggerEnemyEventGA(EnemySlotView Enemy, Events events)
    {
        gameEvent = events;
        enemySlotView = Enemy;
    }
}
