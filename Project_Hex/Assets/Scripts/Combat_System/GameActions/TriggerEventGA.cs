using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEventGA : GameAction
{
    public Events gameEvent;
    public Card card;

    public TriggerEventGA(Events events, Card Card = null)
    {
        gameEvent = events;
        card = Card;
    }
}
