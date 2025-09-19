using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;

public class ChoiceEffect : Effect
{
    public DynamicCondition DynamicCondition;
    [field: SerializeReference, SR] public Effect EffectOnTrue;
    [field: SerializeReference, SR] public Effect EffectOnFalse;
    public int Value;
    public DynamicAmount DynamicAmount;
    public override GameAction GetGameAction()
    {
        TestConditionGA testConditionGA = new(DynamicCondition, EffectOnTrue, EffectOnFalse, Value, DynamicAmount);
        return testConditionGA;
    }

    public ChoiceEffect() { }

    public ChoiceEffect(int value, ActionnerType ActionnerType, Events Event, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmount dynamicAmount, EventReference sfx, DynamicCondition dynamicCondition, Effect effectOnTrue, Effect effectOnFalse)
    {
        Value = value;
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
        DynamicCondition = dynamicCondition;
        EffectOnTrue = effectOnTrue;
        EffectOnFalse = effectOnFalse;
        SFX = sfx;
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

        return new ChoiceEffect(
            Value,
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
            SFX,
            DynamicCondition,
            EffectOnTrue,
            EffectOnFalse
        );
    }
}
