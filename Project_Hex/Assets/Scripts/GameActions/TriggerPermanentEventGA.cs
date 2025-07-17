using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPermanentEventGA : GameAction
{
    public PermanentView permanentView;
    public Events gameEvent;

    public TriggerPermanentEventGA(PermanentView PermanentView,Events events)
    {
        permanentView = PermanentView;
        gameEvent = events;
    }
}
