using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEventGA : GameAction
{
    public Events gameEvent;
    public CardView cardview;
    public PermanentView permanentView;
    public EnemySlotView enemySlotView;

    public TriggerEventGA(Events events, CardView Card = null, PermanentView PermanentView = null, EnemySlotView EnemySlotView = null)
    {
        gameEvent = events;
        cardview = Card;
        permanentView = PermanentView;
        enemySlotView = EnemySlotView;
    }
}
