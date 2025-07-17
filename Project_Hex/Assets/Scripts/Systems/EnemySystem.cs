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
        ActionSystem.AttachPerformer<_ShieldEnemyGA>(ShieldEnemyPerformer);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyTurnGA>();
        ActionSystem.DetachPerformer<_AttackPlayerGA>();
        ActionSystem.DetachPerformer<_HealEnemyGA>();
        ActionSystem.DetachPerformer<_ShieldEnemyGA>();
    }


    // Performers
    private IEnumerator EnemyTurnPerformer(EnemyTurnGA enemyTurnGA)
    {
        foreach (var slot in enemyView.Slots)
        {
            if (slot.IntentAction == null) continue;
            if (slot.IntentAction.Events == Events.Instant)
            {
                ActionSystem.Instance.AddReaction(slot.IntentAction.GetGameAction());
                slot.UpdateIntent();
            }

        }
        yield return null;
    }

    private IEnumerator ShieldEnemyPerformer(_ShieldEnemyGA shieldEnemyGA)
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
                    ActionSystem.Instance.AddReaction(new ShieldGA(shieldEnemyGA.Amount, playerTargets, null));

                if (enemyTargets != null && enemyTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new ShieldGA(shieldEnemyGA.Amount, null, enemyTargets));
            }
        }
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
}
