using UnityEngine;
using System;
using System.Collections.Generic;

public class DrawCardsEffect : Effect
{
    [Header("Effect Param")]
    [SerializeField] public int drawAmount;
    [SerializeField] public DynamicAmount DynamicAmount;

    public override GameAction GetGameAction()
    {
        if (DynamicAmount != DynamicAmount.NULL)
        {
            drawAmount = TargetSystem.Instance.GetDynamicAmount(DynamicAmount);
        }
        DrawCardsGA drawCardsGA = new(drawAmount);
        return drawCardsGA;
    }
    public DrawCardsEffect(){}

    public DrawCardsEffect(int Amount, ActionnerType ActionnerType, Events Event, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmount dynamicAmount)
    {
        drawAmount = Amount;
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

        return new DrawCardsEffect(
            drawAmount,
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
