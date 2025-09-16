using System;
using System.Collections.Generic;
using UnityEngine;

public class ScryEffect : Effect
{
    [Header("Effect Param")]
    [SerializeField] public int ScryAmount;
    [SerializeField] public DynamicAmount DynamicAmount;

    public override GameAction GetGameAction()
    {
        if (DynamicAmount != DynamicAmount.NULL)
        {
            ScryAmount = TargetSystem.Instance.GetDynamicAmount(DynamicAmount);
        }
        ScryGA scryGA = new(ScryAmount);
        return scryGA;
    }
    public ScryEffect(){}

    public ScryEffect(int Amount, ActionnerType ActionnerType, Events Event, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmount dynamicAmount)
    {
        ScryAmount = Amount;
        Events = Event;
        actionnerType = ActionnerType;
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

        return new ScryEffect(
            ScryAmount,
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
