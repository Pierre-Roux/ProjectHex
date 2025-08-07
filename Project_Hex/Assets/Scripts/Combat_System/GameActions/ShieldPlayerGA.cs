using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPlayerGA : GameAction
{
    public TargetMode TargetMode;

    public ShieldPlayerGA(TargetMode targetMode)
    {
        TargetMode = targetMode;
    }
}
