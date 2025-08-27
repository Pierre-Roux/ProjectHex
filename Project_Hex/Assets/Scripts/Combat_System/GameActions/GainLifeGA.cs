using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainLifeGA : GameAction
{
    public int Amount { get; set; }
    public bool passive;
    public PermaTypes permaTypes;
    public TargetMode targetMode;
    public List<PermanentView> Targets_Player { get; set; }
    public List<EnemySlotView> Targets_Enemy { get; set; }

    public GainLifeGA(int amount, bool Passive, PermaTypes PermaTypes, List<PermanentView> targets_Player = null, List<EnemySlotView> targets_Enemy = null, TargetMode TargetMode = TargetMode.Self)
    {
        Amount = amount;
        passive = Passive;
        permaTypes = PermaTypes;
        Targets_Player = targets_Player;
        Targets_Enemy = targets_Enemy;
        targetMode = TargetMode;
    }
}
