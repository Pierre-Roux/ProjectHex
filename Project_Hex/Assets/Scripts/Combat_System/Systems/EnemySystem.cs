using System.Collections;
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

        ActionSystem.SubscribeReaction<AttackPlayerGA>(BeforeAttackPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<HealEnemyGA>(BeforeHealPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<ShieldEnemyGA>(BeforeShieldPreReaction, ReactionTiming.PRE);

        ActionSystem.SubscribeReaction<EnemyTurnGA>(PerformIntentConstruct, ReactionTiming.POST);
        
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyTurnGA>();
        ActionSystem.DetachPerformer<AttackPlayerGA>();
        ActionSystem.DetachPerformer<HealEnemyGA>();
        ActionSystem.DetachPerformer<ShieldEnemyGA>();

        ActionSystem.UnsubscribeReaction<AttackPlayerGA>(BeforeAttackPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<HealEnemyGA>(BeforeHealPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<ShieldEnemyGA>(BeforeShieldPreReaction, ReactionTiming.PRE);

        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(PerformIntentConstruct, ReactionTiming.POST);
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

            if (attackPlayerGA.TargetMode == TargetMode.Manual)
            {

            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(attackPlayerGA.TargetMode, attackPlayerGA.Actionner);

                if (playerTargets != null && playerTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new DealDamageGA(attackPlayerGA.Damage, playerTargets, null));

                if (enemyTargets != null && enemyTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new DealDamageGA(attackPlayerGA.Damage, null, enemyTargets));
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

            if (healEnemyGA.TargetMode == TargetMode.Manual)
            {

            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(healEnemyGA.TargetMode, healEnemyGA.Actionner);

                if (playerTargets != null && playerTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new HealGA(healEnemyGA.HealAmount, playerTargets, null));

                if (enemyTargets != null && enemyTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new HealGA(healEnemyGA.HealAmount, null, enemyTargets));
            }
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

            if (shieldEnemyGA.TargetMode == TargetMode.Manual)
            {

            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(shieldEnemyGA.TargetMode, shieldEnemyGA.Actionner);

                if (playerTargets != null && playerTargets.Count > 0)
                {
                    ShieldGA shieldGA = new ShieldGA(playerTargets, null);
                    shieldGA.Actionner = shieldEnemyGA.Actionner;
                    ActionSystem.Instance.AddReaction(shieldGA);
                }

                if (enemyTargets != null && enemyTargets.Count > 0)
                {
                    ShieldGA shieldGA = new ShieldGA(null, enemyTargets);
                    shieldGA.Actionner = shieldEnemyGA.Actionner;
                    ActionSystem.Instance.AddReaction(shieldGA);
                }
            }
        }
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

    private void PerformIntentConstruct(EnemyTurnGA enemyTurnGA)
    {
        if (enemyView.IntentConstructs == null || enemyView.IntentConstructs.Count == 0)
        {
            Debug.LogWarning("No intent constructs found.");
            return;
        }

        if (enemyView.ConstructSequence == null || enemyView.ConstructSequence.Count == 0)
        {
            Debug.LogWarning("No construct sequence defined.");
            return;
        }

        if (enemyView.sequenceIndex >= enemyView.ConstructSequence.Count)
        {
            if (enemyView.LoopingSequence)
            {
                enemyView.sequenceIndex = 0;
            }
            else
            {
                return;
            }
        }

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

                Debug.Log($"✅ Performed intent '{currentKey}'");
            }
        }

        enemyView.sequenceIndex++;
        CombatSystem.Instance.CurrentTurn++;
    }
}
