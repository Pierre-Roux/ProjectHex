using System;
using System.Collections.Generic;

public class _AttackPlayerGA : GameAction
{
    public TargetMode TargetMode;
    public int Damage;
    public _AttackPlayerGA(int damage, TargetMode targetMode)
    {
        Damage = damage;
        TargetMode = targetMode;
    }
}
