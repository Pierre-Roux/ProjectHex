using System;
using System.Collections.Generic;
using UnityEngine;

public class AlterPowerEffect : Effect
{
    [Header("Effect Param")]

    [SerializeField] public int alterAmount;
    [SerializeField] public DynamicAmount DynamicAmount;
    [SerializeField] public TargetMode targetMode;
    [SerializeField] public bool passive;
    [SerializeField] public PermaTypes permaTypes;

    [Header("For Manual Target only")]
    [SerializeField] private int targetNumber;

    public AlterPowerEffect() { }

    public AlterPowerEffect(int AlterAmount, TargetMode TargetMode, int TargetNumber, ActionnerType ActionnerType, Events Event, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool Passive, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, PermaTypes PermaTypes, DynamicAmount dynamicAmount)
    {
        alterAmount = AlterAmount;
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
                alterAmount = TargetSystem.Instance.GetDynamicAmount(DynamicAmount);
            }
            if (passive)
            {
                AlterPowerGA alterPowerGA = new(alterAmount, passive, permaTypes, null, null, targetMode);
                return alterPowerGA;
            }
            else
            {
                if (targetMode == TargetMode.Manual)
                {
                    AlterPowerGA alterPowerGA = new(alterAmount, passive, permaTypes, null);
                    StartManualTargetingGA startManualTargetingGA = new(alterPowerGA, targetNumber, this);
                    return startManualTargetingGA;
                }
                else if (targetMode == TargetMode.EffectParent_Targets)
                {
                    AlterPowerGA alterPowerGA = new(alterAmount, passive, permaTypes, ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy);
                    return alterPowerGA;
                }
                else
                {
                    var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetMode, null);

                    TargetForLinked_Player = playerTargets;
                    TargetForLinked_Enemy = enemyTargets;

                    AlterPowerGA alterPowerGA = new(alterAmount, passive, permaTypes, playerTargets, enemyTargets);
                    return alterPowerGA;
                }
            }

        }
        else
        {
            if (actionnerType == ActionnerType.ENEMY && Actionner != null)
            {
                if (DynamicAmount != DynamicAmount.NULL)
                {
                    alterAmount = TargetSystem.Instance.GetDynamicAmount(DynamicAmount,null,Actionner.GetComponent<EnemySlotView>());
                }
                if (passive)
                {
                    EnemyAlterPowerGA enemyAlterPowerGA = new(alterAmount, passive, permaTypes, null, null, targetMode);
                    enemyAlterPowerGA.Actionner = Actionner;
                    return enemyAlterPowerGA;
                }
                else
                {
                    if (targetMode == TargetMode.Manual)
                    {
                        EnemyAlterPowerGA enemyAlterPowerGA = new(alterAmount, passive, permaTypes, null, null, targetMode);
                        enemyAlterPowerGA.Actionner = Actionner;
                        StartManualTargetingGA startManualTargetingGA = new(enemyAlterPowerGA, targetNumber, this);
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

                        EnemyAlterPowerGA enemyAlterPowerGA = new(alterAmount, passive, permaTypes, playerTargets, enemyTargets);
                        enemyAlterPowerGA.Actionner = Actionner;
                        return enemyAlterPowerGA;
                    }
                }

            }
            else if (actionnerType == ActionnerType.PLAYER && Actionner != null)
            {
                if (DynamicAmount != DynamicAmount.NULL)
                {
                    alterAmount = TargetSystem.Instance.GetDynamicAmount(DynamicAmount,Actionner.GetComponent<PermanentView>(),null);
                }
                if (passive)
                {
                    PlayerAlterPowerGA playerAlterPowerGA = new(alterAmount, passive, permaTypes, null, null, targetMode);
                    playerAlterPowerGA.Actionner = Actionner;
                    return playerAlterPowerGA;
                }
                else
                {
                    if (targetMode == TargetMode.Manual)
                    {
                        PlayerAlterPowerGA playerAlterPowerGA = new(alterAmount, passive, permaTypes, null, null, targetMode);
                        playerAlterPowerGA.Actionner = Actionner;
                        StartManualTargetingGA startManualTargetingGA = new(playerAlterPowerGA, targetNumber, this);
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

                        PlayerAlterPowerGA playerAlterPowerGA = new(alterAmount, passive, permaTypes, playerTargets, enemyTargets);
                        playerAlterPowerGA.Actionner = Actionner;
                        return playerAlterPowerGA;
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

        return new AlterPowerEffect(
            alterAmount,
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
