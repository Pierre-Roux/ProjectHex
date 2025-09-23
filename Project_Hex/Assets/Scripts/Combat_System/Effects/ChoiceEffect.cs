using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;

public class ChoiceEffect : Effect
{
    [field: SerializeReference, SR] public Effect EffectOnTrue;
    [field: SerializeReference, SR] public Effect EffectOnFalse;

    public override GameAction GetGameAction()
    {
        TestConditionGA testConditionGA = new(DynamicCondition, EffectOnTrue, EffectOnFalse, TestValue, TestDynamicAmount);
        return testConditionGA;
    }

    public ChoiceEffect() { }

    public ChoiceEffect(int value,PermaTypes testType, ActionnerType ActionnerType, Events Event, bool cancelOnDeath, GameObject actionner, CardView cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmount dynamicAmount, EventReference sfx, DynamicCondition dynamicCondition, Effect effectOnTrue, Effect effectOnFalse)
    {
        Events = Event;
        TestValue = value;
        TestDynamicAmount = dynamicAmount;
        TestType = testType;
        CancelOnDeath = cancelOnDeath;
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
        TestDynamicAmount = dynamicAmount;
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
            TestValue,
            TestType,
            actionnerType,
            Events,
            CancelOnDeath,
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
            TestDynamicAmount,
            SFX,
            DynamicCondition,
            EffectOnTrue,
            EffectOnFalse
        );
    }
}
