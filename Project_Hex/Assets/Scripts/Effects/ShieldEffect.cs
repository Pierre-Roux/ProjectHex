using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEffect : Effect
{
    [SerializeField] private int amount;
    [SerializeField] private TargetMode targetMode;
    [SerializeField] private int targetNumber;

    public ShieldEffect(){}

    public ShieldEffect(int Amount, TargetMode TargetMode, int TargetNumber, ActionnerType ActionnerType, Events Event, GameObject actionner, Card cardActionner)
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
                ShieldGA shieldGA = new(amount, null, null);
                StartManualTargetingGA startManualTargetingGA = new(shieldGA, targetNumber);
                return startManualTargetingGA;
            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetMode, null);
                ShieldGA shieldGA = new(amount, playerTargets, enemyTargets);
                return shieldGA;
            }
        }
        // SI PERMANENT
        else
        {
            // SI ENEMY
            if (actionnerType == ActionnerType.ENEMY)
            {
                _ShieldEnemyGA _shieldEnemyGA = new(amount, targetMode);
                _shieldEnemyGA.Actionner = Actionner;
                return _shieldEnemyGA;
            }
            // SI PLAYER
            else if (actionnerType == ActionnerType.PLAYER)
            {
                ShieldPlayerGA shieldPlayerGA = new(amount, targetMode);
                shieldPlayerGA.Actionner = Actionner;
                return shieldPlayerGA;
            }
            //NEVER
            else {return null;}
        }
    }

    public override Effect Clone()
    {
        return new ShieldEffect(amount, targetMode, targetNumber, actionnerType, Events,Actionner,CardActionner);
    }
}
