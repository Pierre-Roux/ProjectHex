using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeLossEffect : Effect
{
    [Header("Effect Param")]

    [SerializeField] public int LifeLossAmount;
    [SerializeField] public DynamicAmount DynamicAmount;
    [SerializeField] public TargetMode targetMode;

    [Header("For Manual Target only")]
    [SerializeField] private int targetNumber;

    public LifeLossEffect(){}

    public LifeLossEffect(int lifeLossAmount, TargetMode TargetMode, int TargetNumber, ActionnerType ActionnerType, Events Event, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmount dynamicAmount)
    {
        LifeLossAmount = lifeLossAmount;
        targetMode = TargetMode;
        targetNumber = TargetNumber;
        actionnerType = ActionnerType;
        CardActionner = cardActionner;
        Events = Event;
        Actionner = actionner;
        Intent_Title = intent_Title;
        number = Number;
        Duration = duration;
        DurationType = durationType;
        TriggerOnDurationEnd = triggerOnDurationEnd;
        LinkedEffect = linkedEffect;
        TargetForLinked_Player = targetForLinked_Player;
        TargetForLinked_Enemy = targetForLinked_Enemy;
        DynamicAmount = dynamicAmount;
    }

    public override GameAction GetGameAction()
    {
        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (DynamicAmount != DynamicAmount.NULL)
            {
                LifeLossAmount = TargetSystem.Instance.GetDynamicAmount(DynamicAmount);
            }
            if (targetMode == TargetMode.Manual)
            {
                LifeLossGA lifeLossGA = new(LifeLossAmount, null, null);
                StartManualTargetingGA startManualTargetingGA = new(lifeLossGA, targetNumber, this);
                return startManualTargetingGA;
            }
            else if (targetMode == TargetMode.EffectParent_Targets)
            {
                LifeLossGA lifeLossGA = new(LifeLossAmount, ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy);
                return lifeLossGA;
            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetMode, null);
                TargetForLinked_Player = playerTargets;
                TargetForLinked_Enemy = enemyTargets;

                LifeLossGA lifeLossGA = new(LifeLossAmount, playerTargets, enemyTargets);
                return lifeLossGA;
            }
        }
        else
        {
            if (actionnerType == ActionnerType.ENEMY && Actionner != null)
            {
                if (DynamicAmount != DynamicAmount.NULL)
                {
                    LifeLossAmount = TargetSystem.Instance.GetDynamicAmount(DynamicAmount,null,Actionner.GetComponent<EnemySlotView>());
                }
                if (targetMode == TargetMode.Manual)
                {
                    EnemyLifeLossGA enemyLifeLossGA = new(LifeLossAmount, null, null);
                    StartManualTargetingGA startManualTargetingGA = new(enemyLifeLossGA, targetNumber, this);
                    return startManualTargetingGA;
                }
                else
                {
                    List<PermanentView> playerTargets;
                    List<EnemySlotView> enemyTargets;

                    if (targetMode == TargetMode.EffectParent_Targets)
                    {
                        playerTargets = ParentEffect.TargetForLinked_Player;
                        enemyTargets = ParentEffect.TargetForLinked_Enemy;
                    }
                    else
                    {
                        (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetMode, Actionner);

                        TargetForLinked_Player = playerTargets;
                        TargetForLinked_Enemy = enemyTargets;
                    }

                    EnemyLifeLossGA enemyLifeLossGA = new(LifeLossAmount, playerTargets, enemyTargets);
                    enemyLifeLossGA.Actionner = Actionner;
                    return enemyLifeLossGA;
                }
            }
            else if (actionnerType == ActionnerType.PLAYER && Actionner != null)
            {
                if (DynamicAmount != DynamicAmount.NULL)
                {
                    LifeLossAmount = TargetSystem.Instance.GetDynamicAmount(DynamicAmount,Actionner.GetComponent<PermanentView>(),null);
                }
                if (targetMode == TargetMode.Manual)
                {
                    PlayerLifeLossGA playerLifeLossGA = new(LifeLossAmount, null, null);
                    StartManualTargetingGA startManualTargetingGA = new(playerLifeLossGA, targetNumber, this);
                    return startManualTargetingGA;
                }
                else
                {
                    List<PermanentView> playerTargets;
                    List<EnemySlotView> enemyTargets;

                    if (targetMode == TargetMode.EffectParent_Targets)
                    {
                        playerTargets = ParentEffect.TargetForLinked_Player;
                        enemyTargets = ParentEffect.TargetForLinked_Enemy;
                    }
                    else
                    {
                        (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetMode, Actionner);

                        TargetForLinked_Player = playerTargets;
                        TargetForLinked_Enemy = enemyTargets;
                    }

                    PlayerLifeLossGA playerLifeLossGA = new(LifeLossAmount, playerTargets, enemyTargets);
                    playerLifeLossGA.Actionner = Actionner;
                    return playerLifeLossGA;
                }
            }
            else
            {
                Debug.LogError("Effect.GetGameAction returned Null");
                return null;
            }
        }
    }

    public override Effect Clone()
    {
        var clonedPlayerTargets = TargetForLinked_Player != null 
            ? new List<PermanentView>(TargetForLinked_Player) 
            : null;

        var clonedEnemyTargets = TargetForLinked_Enemy != null 
            ? new List<EnemySlotView>(TargetForLinked_Enemy) 
            : null;

        Effect clonedLinked = LinkedEffect != null ? LinkedEffect.Clone() : null;

        return new LifeLossEffect(
            LifeLossAmount,
            targetMode,
            targetNumber,
            actionnerType,
            Events,
            Actionner,
            CardActionner,
            Intent_Title,
            number,
            Duration,
            DurationType,
            TriggerOnDurationEnd,
            clonedLinked,
            clonedPlayerTargets,
            clonedEnemyTargets,
            DynamicAmount
        );
    }
}
