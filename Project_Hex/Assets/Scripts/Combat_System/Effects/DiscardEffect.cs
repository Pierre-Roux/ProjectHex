using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class DiscardEffect : Effect
{
    [Header("Effect Param")]
    [SerializeField] public int DiscardAmount;
    [SerializeField] public DynamicAmount DynamicAmount;
    [SerializeField] public bool DiscardAll;

    private bool ConditionTested = false;
    public override GameAction GetGameAction()
    {
        if (DiscardAll)
        {
            DiscardAllCardsGA discardAllCardsGA = new(true);
            if (AudioManager.Instance.IsValid(SFX)){ discardAllCardsGA.SFX = SFX; }
            return discardAllCardsGA;
        }
        else
        {
            if (!ConditionTested)
            {
                DiscardEffect DiscardallEffect = (DiscardEffect)this.Clone();
                DiscardallEffect.DiscardAll = true;

                DiscardEffect DiscardManuEffect = (DiscardEffect)this.Clone();
                DiscardManuEffect.ConditionTested = true;

                TestConditionGA testConditionGA = new(DynamicCondition.ValueSupOrEqualsToDynamicAmount,DiscardallEffect,DiscardManuEffect, DiscardAmount,DynamicAmount.CardsInHand_Count);
                return testConditionGA;
            }
            else
            {
                if (DynamicAmount != DynamicAmount.NULL)
                {
                    DiscardAmount = TargetSystem.Instance.GetDynamicAmount(DynamicAmount);
                }
                DiscardCardGA discardCardGA = new(new List<CardView>());
                if (AudioManager.Instance.IsValid(SFX)){ discardCardGA.SFX = SFX; }
                StartCardTargetingGA startCardTargetingGA = new(discardCardGA,DiscardAmount);
                return startCardTargetingGA;
            }
        }
    }
    public DiscardEffect(){}

    public DiscardEffect(int Amount, ActionnerType ActionnerType, Events Event, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmount dynamicAmount, bool discardAll, EventReference sfx, bool conditionTested)
    {
        DiscardAmount = Amount;
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
        DiscardAll = discardAll;
        SFX = sfx;
        ConditionTested = conditionTested;
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

        return new DiscardEffect(
            DiscardAmount,
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
            DiscardAll,
            SFX,
            ConditionTested
        );
    }
}
