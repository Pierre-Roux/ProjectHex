using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShieldEffect : Effect
{
    [Header("Effect Param")]
    [SerializeField] public TargetMode targetMode;

    [Header("For Manual Target only")]
    [SerializeField] private int targetNumber;

    public ShieldEffect(){}

    public ShieldEffect(TargetMode TargetMode, int TargetNumber, ActionnerType ActionnerType, Events Event, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy)
    {
        targetMode = TargetMode;
        targetNumber = TargetNumber;
        actionnerType = ActionnerType;
        Events = Event;
        Actionner = actionner;
        CardActionner = cardActionner;
        Intent_Title = intent_Title;
        number = Number;
        Duration = duration;
        DurationType = durationType;
        TriggerOnDurationEnd = triggerOnDurationEnd;
        LinkedEffect = linkedEffect;
        TargetForLinked_Player = targetForLinked_Player;
        TargetForLinked_Enemy = targetForLinked_Enemy;
    }

    public override GameAction GetGameAction()
    {
        // SI CARTE
        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (targetMode == TargetMode.Manual)
            {
                ShieldGA shieldGA = new(null, null);
                StartManualTargetingGA startManualTargetingGA = new(shieldGA, targetNumber,this);
                return startManualTargetingGA;
            }
            else if (targetMode == TargetMode.EffectParent_Targets)
            {
                ShieldGA shieldGA = new(ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy);
                return shieldGA;
            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetMode, null);
                TargetForLinked_Player = playerTargets;
                TargetForLinked_Enemy = enemyTargets;

                ShieldGA shieldGA = new(playerTargets, enemyTargets);
                return shieldGA;
            }
        }
        // SI PERMANENT
        else
        {
            // SI ENEMY
            if (actionnerType == ActionnerType.ENEMY && Actionner != null)
            {
                if (targetMode == TargetMode.Manual)
                {
                    ShieldEnemyGA shieldEnemyGA = new(null, null);
                    StartManualTargetingGA startManualTargetingGA = new(shieldEnemyGA, targetNumber,this);
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

                    ShieldEnemyGA shieldEnemyGA = new(playerTargets, enemyTargets);
                    shieldEnemyGA.Actionner = Actionner;
                    return shieldEnemyGA;
                }
            }
            // SI PLAYER
            else if (actionnerType == ActionnerType.PLAYER && Actionner != null)
            {
                if (targetMode == TargetMode.Manual)
                {
                    ShieldPlayerGA shieldPlayerGA = new(null, null);
                    StartManualTargetingGA startManualTargetingGA = new(shieldPlayerGA, targetNumber,this);
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

                    ShieldPlayerGA shieldPlayerGA = new(playerTargets, enemyTargets);
                    shieldPlayerGA.Actionner = Actionner;
                    return shieldPlayerGA;
                }
            }
            // NEVER
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

        return new ShieldEffect(
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
            clonedEnemyTargets
        );
    }

}
