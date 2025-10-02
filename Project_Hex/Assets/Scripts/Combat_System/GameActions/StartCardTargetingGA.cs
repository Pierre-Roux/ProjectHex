using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCardTargetingGA : GameAction
{
    public GameAction ActionToRealiseAfterTargetting;
    public int TargetNumber;
    public List<TargetLimitationInfo> TargetLimitations;

    public StartCardTargetingGA(GameAction actionToRealiseAfterTargetting, int targetNumber, List<TargetLimitationInfo> targetLimitations = null)
    {
        ActionToRealiseAfterTargetting = actionToRealiseAfterTargetting;
        TargetNumber = targetNumber;
        TargetLimitations = targetLimitations;
    }
}
