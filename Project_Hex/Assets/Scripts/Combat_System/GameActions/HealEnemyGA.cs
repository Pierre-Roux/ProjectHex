using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEnemyGA : GameAction
{
    public TargetMode TargetMode;
    public int HealAmount;

    public HealEnemyGA(int healAmount, TargetMode targetMode)
    {
        HealAmount = healAmount;
        TargetMode = targetMode;
    }
}
