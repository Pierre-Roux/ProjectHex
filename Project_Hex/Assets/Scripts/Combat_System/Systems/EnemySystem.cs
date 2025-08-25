using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemySystem : Singleton<EnemySystem>
{
    public EnemyView enemyView;

    void OnEnable()
    {
        ActionSystem.AttachPerformer<EnemyTurnGA>(EnemyTurnPerformer);
        ActionSystem.AttachPerformer<AttackPlayerGA>(AttackPlayerPerformer);
        ActionSystem.AttachPerformer<HealEnemyGA>(HealEnemyPerformer);
        ActionSystem.AttachPerformer<ShieldEnemyGA>(ShieldEnemyPerformer);
        ActionSystem.AttachPerformer<EnemyAlterPowerGA>(AlterPlayerPerformer);
        ActionSystem.AttachPerformer<SpawnConstructGA>(PerformIntentConstructPerformer);

        ActionSystem.SubscribeReaction<AttackPlayerGA>(BeforeAttackPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<HealEnemyGA>(BeforeHealPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<ShieldEnemyGA>(BeforeShieldPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyAlterPowerGA>(BeforeAlterPreReaction, ReactionTiming.PRE);
        
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyTurnGA>();
        ActionSystem.DetachPerformer<AttackPlayerGA>();
        ActionSystem.DetachPerformer<HealEnemyGA>();
        ActionSystem.DetachPerformer<ShieldEnemyGA>();
        ActionSystem.DetachPerformer<EnemyAlterPowerGA>();
        ActionSystem.DetachPerformer<SpawnConstructGA>();

        ActionSystem.UnsubscribeReaction<AttackPlayerGA>(BeforeAttackPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<HealEnemyGA>(BeforeHealPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<ShieldEnemyGA>(BeforeShieldPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyAlterPowerGA>(BeforeAlterPreReaction, ReactionTiming.PRE);
    }


    // Performers
    private IEnumerator EnemyTurnPerformer(EnemyTurnGA enemyTurnGA)
    {
        foreach (var enemySlotView in CombatSystem.Instance.Enemy_Permanents)
        {
            if (enemySlotView.IntentAction == null) continue;
            if (enemySlotView.IntentAction.Events == Events.EnemyTurn)
            {
                ActionSystem.Instance.AddReaction(enemySlotView.IntentAction.GetGameAction());
                enemySlotView.UpdateIntent();
            }
        }

        EndEnemyTurnGA endEnemyTurnGA = new();
        ActionSystem.Instance.AddReaction(endEnemyTurnGA);
        yield return null;
    }

    private IEnumerator AttackPlayerPerformer(AttackPlayerGA attackPlayerGA)
    {
        if (attackPlayerGA.Actionner != null)
        {
            EnemySlotView Attacker = attackPlayerGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y - 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            if (attackPlayerGA.playerTargets != null && attackPlayerGA.playerTargets.Count > 0)
            {
                int DamageAmount;
                if (Attacker.isInvoc)
                {
                    DamageAmount = attackPlayerGA.Damage + Attacker.BonusPower + CombatSystem.Instance.EnemyGeneralPower + CombatSystem.Instance.Invoc_EnemyGeneralPower + CombatSystem.Instance.Invoc_GeneralPower;;
                }
                else
                {
                    DamageAmount = attackPlayerGA.Damage + Attacker.BonusPower + CombatSystem.Instance.EnemyGeneralPower;
                }
                ActionSystem.Instance.AddReaction(new DealDamageGA(DamageAmount, attackPlayerGA.playerTargets, null));
            }
                
            if (attackPlayerGA.enemyTargets != null && attackPlayerGA.enemyTargets.Count > 0)
            {
                int DamageAmount;
                if (Attacker.isInvoc)
                {
                    DamageAmount = attackPlayerGA.Damage + Attacker.BonusPower + CombatSystem.Instance.EnemyGeneralPower + CombatSystem.Instance.Invoc_EnemyGeneralPower + CombatSystem.Instance.Invoc_GeneralPower;;
                }
                else
                {
                    DamageAmount = attackPlayerGA.Damage + Attacker.BonusPower + CombatSystem.Instance.EnemyGeneralPower;
                }
                ActionSystem.Instance.AddReaction(new DealDamageGA(DamageAmount, null, attackPlayerGA.enemyTargets));
            }
        }
    }

    private IEnumerator HealEnemyPerformer(HealEnemyGA healEnemyGA)
    {
        if (healEnemyGA.Actionner != null)
        {
            EnemySlotView Attacker = healEnemyGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y - 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            if (healEnemyGA.playerTargets != null && healEnemyGA.playerTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new HealGA(healEnemyGA.HealAmount, healEnemyGA.playerTargets, null));

            if (healEnemyGA.enemyTargets != null && healEnemyGA.enemyTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new HealGA(healEnemyGA.HealAmount, null, healEnemyGA.enemyTargets));
        }
    }

    private IEnumerator ShieldEnemyPerformer(ShieldEnemyGA shieldEnemyGA)
    {
        if (shieldEnemyGA.Actionner != null)
        {
            EnemySlotView Attacker = shieldEnemyGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y - 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            if (shieldEnemyGA.playerTargets != null && shieldEnemyGA.playerTargets.Count > 0)
            {
                ShieldGA shieldGA = new ShieldGA(shieldEnemyGA.playerTargets, null);
                shieldGA.Actionner = shieldEnemyGA.Actionner;
                ActionSystem.Instance.AddReaction(shieldGA);
            }

            if (shieldEnemyGA.enemyTargets != null && shieldEnemyGA.enemyTargets.Count > 0)
            {
                ShieldGA shieldGA = new ShieldGA(null, shieldEnemyGA.enemyTargets);
                shieldGA.Actionner = shieldEnemyGA.Actionner;
                ActionSystem.Instance.AddReaction(shieldGA);
            }
        }
    }

    private IEnumerator AlterPlayerPerformer(EnemyAlterPowerGA enemyAlterPowerGA)
    {
        if (enemyAlterPowerGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyAlterPowerGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);
            if (enemyAlterPowerGA.passive)
            {
                ActionSystem.Instance.AddReaction(new AlterPowerGA(enemyAlterPowerGA.Amount, enemyAlterPowerGA.passive, enemyAlterPowerGA.permaTypes, null, null));
            }
            else
            {
                if (enemyAlterPowerGA.playerTargets != null && enemyAlterPowerGA.playerTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new AlterPowerGA(enemyAlterPowerGA.Amount, enemyAlterPowerGA.passive, enemyAlterPowerGA.permaTypes, enemyAlterPowerGA.playerTargets, null));

                if (enemyAlterPowerGA.enemyTargets != null && enemyAlterPowerGA.enemyTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new AlterPowerGA(enemyAlterPowerGA.Amount, enemyAlterPowerGA.passive, enemyAlterPowerGA.permaTypes, null, enemyAlterPowerGA.enemyTargets));
            }            
        }
    }

    private IEnumerator PerformIntentConstructPerformer(SpawnConstructGA spawnConstructGA)
    {
        if(!CombatSystem.Instance.Win)
        {
            if (enemyView.IntentConstructs != null || enemyView.IntentConstructs.Count != 0)
            {
                if (enemyView.ConstructSequence != null || enemyView.ConstructSequence.Count != 0)
                {

                    bool SequenceFinished = false;

                    if (enemyView.sequenceIndex >= enemyView.ConstructSequence.Count)
                    {
                        if (enemyView.LoopingSequence)
                        {
                            enemyView.sequenceIndex = 0;
                        }
                        else
                        {
                            SequenceFinished = true;
                        }
                    }

                    if (!SequenceFinished)
                    {
                        string currentKey = enemyView.ConstructSequence[enemyView.sequenceIndex];
                        if (currentKey != "")
                        {
                            IntentConstruct selected = enemyView.IntentConstructs.Find(ic => ic.number == currentKey);

                            if (selected == null)
                            {
                                Debug.LogWarning($"No IntentConstruct found for key '{currentKey}'");
                            }
                            else
                            {
                                foreach (EnemyPermanentData data in selected.EnemyData)
                                {
                                    EnemySlotViewCreator.Instance.CreateEnemySlotViewCreator(data, data.permanentType, false, enemyView);
                                }
                            }
                        }
                    }
                }
            }
        }

        enemyView.sequenceIndex++;
        yield return null;
    }

    // REACTIONS

    private void BeforeAttackPreReaction(AttackPlayerGA attackPlayerGA)
    {
        EnemySlotView Attacker = attackPlayerGA.Actionner.GetComponent<EnemySlotView>();
        Attacker.SetPosition(Attacker.transform.position);
    }

    private void BeforeHealPreReaction(HealEnemyGA healEnemyGA)
    {
        EnemySlotView Attacker = healEnemyGA.Actionner.GetComponent<EnemySlotView>();
        Attacker.SetPosition(Attacker.transform.position);
    }

    private void BeforeShieldPreReaction(ShieldEnemyGA shieldEnemyGA)
    {
        EnemySlotView Attacker = shieldEnemyGA.Actionner.GetComponent<EnemySlotView>();
        Attacker.SetPosition(Attacker.transform.position);
    }

    private void BeforeAlterPreReaction(EnemyAlterPowerGA enemyAlterPowerGA)
    {
        EnemySlotView Attacker = enemyAlterPowerGA.Actionner.GetComponent<EnemySlotView>();
        Attacker.SetPosition(Attacker.transform.position);
    }
}
