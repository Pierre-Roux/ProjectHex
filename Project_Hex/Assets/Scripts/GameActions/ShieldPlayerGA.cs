using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPlayerGA : GameAction
{
    public TargetMode TargetMode;
    public int Amount;

    public ShieldPlayerGA(int amount, TargetMode targetMode)
    {
        Amount = amount;
        TargetMode = targetMode;
    }
}
