using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEnemyGA : GameAction
{
    public TargetMode TargetMode;
    public int Damage;
    public AttackEnemyGA(int damage, TargetMode targetMode)
    {
        Damage = damage;
        TargetMode = targetMode;
    }
}
