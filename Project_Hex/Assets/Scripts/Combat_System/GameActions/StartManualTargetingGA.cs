using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManualTargetingGA : GameAction
{
    public GameAction ActionToRealiseAfterTargetting;
    public int TargetNumber;
    public Effect EffectRef;
    public StartManualTargetingGA(GameAction actionToRealiseAfterTargetting, int targetNumber, Effect effectRef = null)
    {
        ActionToRealiseAfterTargetting = actionToRealiseAfterTargetting;
        TargetNumber = targetNumber;
        EffectRef = effectRef;
    }
}
