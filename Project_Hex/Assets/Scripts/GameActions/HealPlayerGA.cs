using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPlayerGA : GameAction
{
    public TargetMode TargetMode;
    public int HealAmount;

    public HealPlayerGA(int healAmount, TargetMode targetMode)
    {
        HealAmount = healAmount;
        TargetMode = targetMode;
    }
}
