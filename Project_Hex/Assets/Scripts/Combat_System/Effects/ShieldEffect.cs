using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEffect : Effect
{
    [Header("Effect Param")]
    [SerializeField] private TargetMode targetMode;

    [Header("For Manual Target only")]
    [SerializeField] private int targetNumber;

    public ShieldEffect(){}

    public ShieldEffect(TargetMode TargetMode, int TargetNumber, ActionnerType ActionnerType, Events Event, GameObject actionner, Card cardActionner)
    {
        targetMode = TargetMode;
        targetNumber = TargetNumber;
        actionnerType = ActionnerType;
        Events = Event;
        Actionner = actionner;
        CardActionner = cardActionner;
    }

    public override GameAction GetGameAction()
    {
        Debug.Log("StartingShield");
        // SI CARTE
        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (targetMode == TargetMode.Manual)
            {
                ShieldGA shieldGA = new(null, null);
                StartManualTargetingGA startManualTargetingGA = new(shieldGA, targetNumber);
                return startManualTargetingGA;
            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetMode, null);
                ShieldGA shieldGA = new(playerTargets, enemyTargets);
                return shieldGA;
            }
        }
        // SI PERMANENT
        else
        {
            // SI ENEMY
            if (actionnerType == ActionnerType.ENEMY)
            {
                ShieldEnemyGA shieldEnemyGA = new(targetMode);
                shieldEnemyGA.Actionner = Actionner;
                return shieldEnemyGA;
            }
            // SI PLAYER
            else if (actionnerType == ActionnerType.PLAYER)
            {
                ShieldPlayerGA shieldPlayerGA = new(targetMode);
                shieldPlayerGA.Actionner = Actionner;
                return shieldPlayerGA;
            }
            //NEVER
            else 
            {
                Debug.Log("ERROR GET GAME ACTION RETURN NULL");
                return null;
            }
        }
    }

    public override Effect Clone()
    {
        return new ShieldEffect(targetMode, targetNumber,actionnerType,Events,Actionner,CardActionner);
    }

}
