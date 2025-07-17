using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _HealEnemyGA : GameAction
{
    public TargetMode TargetMode;
    public int HealAmount;

    public _HealEnemyGA(int healAmount, TargetMode targetMode)
    {
        HealAmount = healAmount;
        TargetMode = targetMode;
    }
}
