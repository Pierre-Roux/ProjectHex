using System;
using System.Collections.Generic;
using UnityEngine;

public class GainHPEffect : Effect
{
    [Header("Effect Param")]

    [SerializeField] public int GainAmount;
    [SerializeField] public DynamicAmount DynamicAmount;
    [SerializeField] public TargetMode targetMode;
    [SerializeField] public bool passive;
    [SerializeField] public PermaTypes permaTypes;

    [Header("For Manual Target only")]
    [SerializeField] private int targetNumber;

    public GainHPEffect() { }

    public GainHPEffect(int gainAmount, TargetMode TargetMode, int TargetNumber, ActionnerType ActionnerType, Events Event, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool Passive, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, PermaTypes PermaTypes, DynamicAmount dynamicAmount)
    {
        GainAmount = gainAmount;
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
        passive = Passive;
        TriggerOnDurationEnd = triggerOnDurationEnd;
        LinkedEffect = linkedEffect;
        TargetForLinked_Player = targetForLinked_Player;
        TargetForLinked_Enemy = targetForLinked_Enemy;
        permaTypes = PermaTypes;
        DynamicAmount = dynamicAmount;
    }

    public override GameAction GetGameAction()
    {
        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (DynamicAmount != DynamicAmount.NULL)
            {
                GainAmount = TargetSystem.Instance.GetDynamicAmount(DynamicAmount);
            }
            if (passive)
            {
                GainLifeGA gainLifeGA = new(GainAmount, passive, permaTypes, null, null, targetMode);
                return gainLifeGA;
            }
            else
            {
                if (targetMode == TargetMode.Manual)
                {
                    GainLifeGA gainLifeGA = new(GainAmount, passive, permaTypes, null);
                    StartManualTargetingGA startManualTargetingGA = new(gainLifeGA, targetNumber, this);
                    return startManualTargetingGA;
                }
                else if (targetMode == TargetMode.EffectParent_Targets)
                {
                    GainLifeGA gainLifeGA = new(GainAmount, passive, permaTypes, ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy);
                    return gainLifeGA;
                }
                else
                {
                    var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetMode, null);

                    TargetForLinked_Player = playerTargets;
                    TargetForLinked_Enemy = enemyTargets;

                    GainLifeGA gainLifeGA = new(GainAmount, passive, permaTypes, playerTargets, enemyTargets);
                    return gainLifeGA;
                }
            }

        }
        else
        {
            if (actionnerType == ActionnerType.ENEMY && Actionner != null)
            {
                if (DynamicAmount != DynamicAmount.NULL)
                {
                    GainAmount = TargetSystem.Instance.GetDynamicAmount(DynamicAmount,null,Actionner.GetComponent<EnemySlotView>());
                }
                if (passive)
                {
                    EnemyGainLifeGA enemyGainLifeGA = new(GainAmount, passive, permaTypes, null, null, targetMode);
                    enemyGainLifeGA.Actionner = Actionner;
                    return enemyGainLifeGA;
                }
                else
                {
                    if (targetMode == TargetMode.Manual)
                    {
                        EnemyGainLifeGA enemyGainLifeGA = new(GainAmount, passive, permaTypes, null, null);
                        StartManualTargetingGA startManualTargetingGA = new(enemyGainLifeGA, targetNumber, this);
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

                        EnemyGainLifeGA enemyGainLifeGA = new(GainAmount, passive, permaTypes, playerTargets, enemyTargets);
                        enemyGainLifeGA.Actionner = Actionner;
                        return enemyGainLifeGA;
                    }
                }

            }
            else if (actionnerType == ActionnerType.PLAYER && Actionner != null)
            {
                if (DynamicAmount != DynamicAmount.NULL)
                {
                    GainAmount = TargetSystem.Instance.GetDynamicAmount(DynamicAmount,Actionner.GetComponent<PermanentView>(),null);
                }
                if (passive)
                {
                    PlayerGainLifeGA playerGainLifeGA = new(GainAmount, passive, permaTypes, null, null, targetMode);
                    playerGainLifeGA.Actionner = Actionner;
                    return playerGainLifeGA;
                }
                else
                {
                    if (targetMode == TargetMode.Manual)
                    {
                        PlayerGainLifeGA playerGainLifeGA = new(GainAmount, passive, permaTypes, null, null);
                        StartManualTargetingGA startManualTargetingGA = new(playerGainLifeGA, targetNumber, this);
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

                        PlayerGainLifeGA playerGainLifeGA = new(GainAmount, passive, permaTypes, playerTargets, enemyTargets);
                        playerGainLifeGA.Actionner = Actionner;
                        return playerGainLifeGA;
                    }
                }
            }
            else
            {
                Debug.Log("Effect.GetGameAction returned Null");
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

        return new GainHPEffect(
            GainAmount,
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
            passive,
            TriggerOnDurationEnd,
            clonedLinked,
            clonedPlayerTargets,
            clonedEnemyTargets,
            permaTypes,
            DynamicAmount
        );
    }
}
