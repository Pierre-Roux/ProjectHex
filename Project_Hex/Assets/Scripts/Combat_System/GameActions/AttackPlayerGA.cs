using System;
using System.Collections.Generic;

public class AttackPlayerGA : GameAction
{
    public TargetMode TargetMode;
    public int Damage;
    public AttackPlayerGA(int damage, TargetMode targetMode)
    {
        Damage = damage;
        TargetMode = targetMode;
    }
}
