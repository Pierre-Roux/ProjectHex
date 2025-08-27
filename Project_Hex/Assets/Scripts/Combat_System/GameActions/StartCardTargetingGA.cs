using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCardTargetingGA : GameAction
{
    public GameAction ActionToRealiseAfterTargetting;
    public int TargetNumber;
    public StartCardTargetingGA(GameAction actionToRealiseAfterTargetting, int targetNumber)
    {
        ActionToRealiseAfterTargetting = actionToRealiseAfterTargetting;
        TargetNumber = targetNumber;
    }
}
