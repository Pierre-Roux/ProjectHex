using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _ShieldEnemyGA : GameAction
{
    public TargetMode TargetMode;
    public int Amount;

    public _ShieldEnemyGA(int amount, TargetMode targetMode)
    {
        Amount = amount;
        TargetMode = targetMode;
    }
}
