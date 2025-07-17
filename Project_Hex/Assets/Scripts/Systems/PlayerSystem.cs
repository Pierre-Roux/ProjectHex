using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PlayerSystem : Singleton<PlayerSystem>
{
    void OnEnable()
    {
        ActionSystem.AttachPerformer<AttackEnemyGA>(AttackEnemyPerformer);
        ActionSystem.AttachPerformer<HealPlayerGA>(HealPlayerPerformer);
        ActionSystem.AttachPerformer<ShieldPlayerGA>(ShieldPlayerPerformer);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<AttackEnemyGA>();
        ActionSystem.DetachPerformer<HealPlayerGA>();
        ActionSystem.DetachPerformer<ShieldPlayerGA>();
    }

    private IEnumerator ShieldPlayerPerformer(ShieldPlayerGA shieldPlayerGA)
    {
        if (shieldPlayerGA.Actionner != null)
        {
            PermanentView Attacker = shieldPlayerGA.Actionner.GetComponent<PermanentView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.position.y, 0.35f);
            if (shieldPlayerGA.TargetMode == TargetMode.Manual)
            {

            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(shieldPlayerGA.TargetMode, shieldPlayerGA.Actionner);

                if (playerTargets != null && playerTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new ShieldGA(shieldPlayerGA.Amount, playerTargets, null));

                if (enemyTargets != null && enemyTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new ShieldGA(shieldPlayerGA.Amount, null, enemyTargets));
            }
        }
    }

    private IEnumerator AttackEnemyPerformer(AttackEnemyGA attackEnemyGA)
    {
        if (attackEnemyGA.Actionner != null)
        {
            PermanentView Attacker = attackEnemyGA.Actionner.GetComponent<PermanentView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.position.y, 0.35f);

            if (attackEnemyGA.TargetMode == TargetMode.Manual)
            {

            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(attackEnemyGA.TargetMode, attackEnemyGA.Actionner);

                if (playerTargets != null && playerTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new DealDamageGA(attackEnemyGA.Damage, playerTargets, null));

                if (enemyTargets != null && enemyTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new DealDamageGA(attackEnemyGA.Damage, null, enemyTargets));
            }
        }
    }

    private IEnumerator HealPlayerPerformer(HealPlayerGA healPlayerGA)
    {
        if (healPlayerGA.Actionner != null)
        {
            PermanentView Attacker = healPlayerGA.Actionner.GetComponent<PermanentView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.position.y, 0.35f);
            if (healPlayerGA.TargetMode == TargetMode.Manual)
            {

            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(healPlayerGA.TargetMode, healPlayerGA.Actionner);

                if (playerTargets != null && playerTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new HealGA(healPlayerGA.HealAmount, playerTargets, null));

                if (enemyTargets != null && enemyTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new HealGA(healPlayerGA.HealAmount, null, enemyTargets));
            }
        }
    }
}
