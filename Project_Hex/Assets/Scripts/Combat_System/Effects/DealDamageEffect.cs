using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DealDamageEffect : Effect
{
    [Header("Effect Param")]

    [SerializeField] public int damageAmount;
    [SerializeField] public DynamicAmount DynamicAmount;
    [SerializeField] public TargetMode targetMode;

    [Header("For Manual Target only")]
    [SerializeField] private int targetNumber;

    public DealDamageEffect(){}

    public DealDamageEffect(int DamageAmount, TargetMode TargetMode, int TargetNumber, ActionnerType ActionnerType, Events Event, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmount dynamicAmount)
    {
        damageAmount = DamageAmount;
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
                damageAmount = TargetSystem.Instance.GetDynamicAmount(DynamicAmount);
            }
            if (targetMode == TargetMode.Manual)
            {
                DealDamageGA dealDamageGA = new(damageAmount, null, null);
                StartManualTargetingGA startManualTargetingGA = new(dealDamageGA, targetNumber, this);
                return startManualTargetingGA;
            }
            else if (targetMode == TargetMode.EffectParent_Targets)
            {
                DealDamageGA dealDamageGA = new(damageAmount, ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy);
                return dealDamageGA;
            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetMode, null);
                TargetForLinked_Player = playerTargets;
                TargetForLinked_Enemy = enemyTargets;

                DealDamageGA dealDamageGA = new(damageAmount, playerTargets, enemyTargets);
                return dealDamageGA;
            }
        }
        else
        {
            if (actionnerType == ActionnerType.ENEMY && Actionner != null)
            {
                if (DynamicAmount != DynamicAmount.NULL)
                {
                    damageAmount = TargetSystem.Instance.GetDynamicAmount(DynamicAmount,null,Actionner.GetComponent<EnemySlotView>());
                }
                if (targetMode == TargetMode.Manual)
                {
                    AttackPlayerGA attackPlayerGA = new(damageAmount, null, null);
                    attackPlayerGA.Actionner = Actionner;
                    StartManualTargetingGA startManualTargetingGA = new(attackPlayerGA, targetNumber, this);
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

                    AttackPlayerGA attackPlayerGA = new(damageAmount, playerTargets, enemyTargets);
                    attackPlayerGA.Actionner = Actionner;
                    return attackPlayerGA;
                }
            }
            else if (actionnerType == ActionnerType.PLAYER && Actionner != null)
            {
                if (DynamicAmount != DynamicAmount.NULL)
                {
                    damageAmount = TargetSystem.Instance.GetDynamicAmount(DynamicAmount,Actionner.GetComponent<PermanentView>(),null);
                }
                if (targetMode == TargetMode.Manual)
                {
                    AttackEnemyGA attackEnemyGA = new(damageAmount, null, null);
                    attackEnemyGA.Actionner = Actionner;
                    StartManualTargetingGA startManualTargetingGA = new(attackEnemyGA, targetNumber, this);
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

                    AttackEnemyGA attackEnemyGA = new(damageAmount, playerTargets, enemyTargets);
                    attackEnemyGA.Actionner = Actionner;
                    return attackEnemyGA;
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

        return new DealDamageEffect(
            damageAmount,
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
