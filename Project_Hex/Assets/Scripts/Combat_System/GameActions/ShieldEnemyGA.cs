using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEnemyGA : GameAction
{
    public TargetMode TargetMode;

    public ShieldEnemyGA(TargetMode targetMode)
    {
        TargetMode = targetMode;
    }
}
