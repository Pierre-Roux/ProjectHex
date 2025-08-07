using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEffect : Effect
{
    [Header("Effect Param")]
    [SerializeField] private int amount;
    [SerializeField] private TargetMode targetMode;

    [Header("For Manual Target only")]
    [SerializeField] private int targetNumber;

    public HealEffect(){}

    public HealEffect(int Amount, TargetMode TargetMode, int TargetNumber, ActionnerType ActionnerType, Events Event, GameObject actionner, Card cardActionner)
    {
        amount = Amount;
        targetMode = TargetMode;
        targetNumber = TargetNumber;
        actionnerType = ActionnerType;
        Events = Event;
        Actionner = actionner;
        CardActionner = cardActionner;
    }

    public override GameAction GetGameAction()
    {
        // SI CARTE
        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (targetMode == TargetMode.Manual)
            {
                HealGA healGA = new(amount, null, null);
                StartManualTargetingGA startManualTargetingGA = new(healGA, targetNumber);
                return startManualTargetingGA;
            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetMode, null);
                HealGA healGA = new(amount, playerTargets, enemyTargets);
                return healGA;
            }
        }
        // SI PERMANENT
        else
        {
            // SI ENEMY
            if (actionnerType == ActionnerType.ENEMY)
            {
                HealEnemyGA healEnemyGA = new(amount, targetMode);
                healEnemyGA.Actionner = Actionner;
                return healEnemyGA;
            }
            // SI PLAYER
            else if (actionnerType == ActionnerType.PLAYER)
            {
                HealPlayerGA healPlayerGA = new(amount, targetMode);
                healPlayerGA.Actionner = Actionner;
                return healPlayerGA;
            }
            //NEVER
            else {return null;}
        }
    }

    public override Effect Clone()
    {
        return new HealEffect(amount, targetMode, targetNumber,actionnerType,Events,Actionner,CardActionner);
    }
}
