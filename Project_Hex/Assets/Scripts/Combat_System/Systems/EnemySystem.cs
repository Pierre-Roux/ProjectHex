using System.Collections;
using DG.Tweening;
using UnityEngine;

public class EnemySystem : Singleton<EnemySystem>
{
    public EnemyView enemyView;

    void OnEnable()
    {
        ActionSystem.AttachPerformer<EnemyTurnGA>(EnemyTurnPerformer);
        ActionSystem.AttachPerformer<_AttackPlayerGA>(AttackPlayerPerformer);
        ActionSystem.AttachPerformer<_HealEnemyGA>(HealEnemyPerformer);

        ActionSystem.SubscribeReaction<_AttackPlayerGA>(BeforeAttackPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<_HealEnemyGA>(BeforeHealPreReaction, ReactionTiming.PRE);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyTurnGA>();
        ActionSystem.DetachPerformer<_AttackPlayerGA>();
        ActionSystem.DetachPerformer<_HealEnemyGA>();

        ActionSystem.UnsubscribeReaction<_AttackPlayerGA>(BeforeAttackPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<_HealEnemyGA>(BeforeHealPreReaction, ReactionTiming.PRE);
    }


    // Performers
    private IEnumerator EnemyTurnPerformer(EnemyTurnGA enemyTurnGA)
    {
        foreach (var enemySlotView in CombatSystem.Instance.Enemy_Permanents)
        {
            if (enemySlotView.IntentAction == null) continue;
            if (enemySlotView.IntentAction.Events == Events.Instant)
            {
                ActionSystem.Instance.AddReaction(enemySlotView.IntentAction.GetGameAction());
                enemySlotView.UpdateIntent();
            }

        }
        yield return null;
    }

    private IEnumerator AttackPlayerPerformer(_AttackPlayerGA attackPlayerGA)
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

    private IEnumerator HealEnemyPerformer(_HealEnemyGA healEnemyGA)
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

    // REACTIONS

    private void BeforeAttackPreReaction(_AttackPlayerGA attackPlayerGA)
    {
        EnemySlotView Attacker = attackPlayerGA.Actionner.GetComponent<EnemySlotView>();
        Attacker.SetPosition(Attacker.transform.position);
    }

    private void BeforeHealPreReaction(_HealEnemyGA healEnemyGA)
    {
        EnemySlotView Attacker = healEnemyGA.Actionner.GetComponent<EnemySlotView>();
        Attacker.SetPosition(Attacker.transform.position);
    }
}
