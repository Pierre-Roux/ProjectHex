using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FMODUnity;

public class HealEffect : Effect
{
    [Header("Effect Param")]
    [SerializeField] public int amount;
    [SerializeField] public DynamicAmount DynamicAmount;
    [SerializeField] public TargetMode targetMode;

    [Header("For Manual Target only")]
    [SerializeField] private int targetNumber;

    public HealEffect(){}

    public HealEffect(int Amount, TargetMode TargetMode, int TargetNumber, ActionnerType ActionnerType, Events Event, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmount dynamicAmount, EventReference sfx)
    {
        amount = Amount;
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
        DynamicAmount = dynamicAmount;
        SFX = sfx;
    }

    public override GameAction GetGameAction()
    {
        // SI CARTE
        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (targetMode == TargetMode.Manual)
            {
                HealGA healGA = new(amount, DynamicAmount, null, null);
                if (AudioManager.Instance.IsValid(SFX)){ healGA.SFX = SFX; }
                StartManualTargetingGA startManualTargetingGA = new(healGA, targetNumber, this);
                return startManualTargetingGA;
            }
            else if (targetMode == TargetMode.EffectParent_Targets)
            {
                HealGA healGA = new(amount, DynamicAmount, ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy);
                if (AudioManager.Instance.IsValid(SFX)){ healGA.SFX = SFX; }
                return healGA;
            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetMode, null);
                TargetForLinked_Player = playerTargets;
                TargetForLinked_Enemy = enemyTargets;

                HealGA healGA = new(amount, DynamicAmount, playerTargets, enemyTargets);
                if (AudioManager.Instance.IsValid(SFX)){ healGA.SFX = SFX; }
                return healGA;
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
                    HealEnemyGA healEnemyGA = new(amount, DynamicAmount, null, null);
                    healEnemyGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)){ healEnemyGA.SFX = SFX; }
                    StartManualTargetingGA startManualTargetingGA = new(healEnemyGA, targetNumber, this);
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

                    HealEnemyGA healEnemyGA = new(amount, DynamicAmount, playerTargets, enemyTargets);
                    healEnemyGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)){ healEnemyGA.SFX = SFX; }
                    return healEnemyGA;
                }
            }
            // SI PLAYER
            else if (actionnerType == ActionnerType.PLAYER && Actionner != null)
            {
                if (targetMode == TargetMode.Manual)
                {
                    HealPlayerGA healPlayerGA = new(amount, DynamicAmount, null, null);
                    healPlayerGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)){ healPlayerGA.SFX = SFX; }
                    StartManualTargetingGA startManualTargetingGA = new(healPlayerGA, targetNumber, this);
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

                    HealPlayerGA healPlayerGA = new(amount, DynamicAmount, playerTargets, enemyTargets);
                    healPlayerGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)){ healPlayerGA.SFX = SFX; }
                    return healPlayerGA;
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

        return new HealEffect(
            amount,
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
            DynamicAmount,
            SFX
        );
    }
}
