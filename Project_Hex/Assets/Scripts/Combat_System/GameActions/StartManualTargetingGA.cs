using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManualTargetingGA : GameAction
{
    public GameAction ActionToRealiseAfterTargetting;
    public List<TargetLimitationInfo> TargetLimitations;
    public int TargetNumber;
    public Effect EffectRef;
    public StartManualTargetingGA(GameAction actionToRealiseAfterTargetting, int targetNumber, Effect effectRef = null, List<TargetLimitationInfo> targetLimitations = null)
    {
        ActionToRealiseAfterTargetting = actionToRealiseAfterTargetting;
        Actionner = actionToRealiseAfterTargetting.Actionner;
        TargetNumber = targetNumber;
        EffectRef = effectRef;
        TargetLimitations = targetLimitations;
    }
}
