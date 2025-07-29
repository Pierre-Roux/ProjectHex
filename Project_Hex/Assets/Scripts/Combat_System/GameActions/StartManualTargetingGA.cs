using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManualTargetingGA : GameAction
{
    public GameAction ActionToRealiseAfterTargetting;
    public int TargetNumber;
    public StartManualTargetingGA(GameAction actionToRealiseAfterTargetting, int targetNumber)
    {
        ActionToRealiseAfterTargetting = actionToRealiseAfterTargetting;
        TargetNumber = targetNumber;
    }
}
