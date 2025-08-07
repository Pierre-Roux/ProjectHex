using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSystem : Singleton<EffectSystem>
{
    // Performers

    void OnEnable()
    {
        ActionSystem.AttachPerformer<DoEffectGA>(PerformEffectPerformer);
        ActionSystem.AttachPerformer<DealDamageGA>(DealDamagePerformer);
        ActionSystem.AttachPerformer<HealGA>(DealHealPerformer); 
        ActionSystem.AttachPerformer<ShieldGA>(DealShieldPerformer); 
        ActionSystem.AttachPerformer<LoseShieldGA>(LoseShieldPerformer); 
        
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<DoEffectGA>();
        ActionSystem.DetachPerformer<DealDamageGA>();
        ActionSystem.DetachPerformer<HealGA>();
        ActionSystem.DetachPerformer<ShieldGA>();
        ActionSystem.DetachPerformer<LoseShieldGA>();
    }

    private IEnumerator AddPerformerToEvent()
    {
        yield return null;
    }

    private IEnumerator PerformEffectPerformer(DoEffectGA doEffectGA)
    {
        GameAction effectAction = doEffectGA.Effect.GetGameAction();
        ActionSystem.Instance.AddReaction(effectAction);
        yield return null;
    }


    private IEnumerator DealDamagePerformer(DealDamageGA dealDamageGA)
    {
        if (dealDamageGA.Targets_Player != null)
        {
            foreach (var target in dealDamageGA.Targets_Player)
            {
                target.TakeDamage(dealDamageGA.Amount);
                yield return new WaitForSeconds(0.15f);
            }
        }

        if (dealDamageGA.Targets_Enemy != null)
        {
            foreach (var target in dealDamageGA.Targets_Enemy)
            {
                target.TakeDamage(dealDamageGA.Amount);
                yield return new WaitForSeconds(0.15f);
            }
        }
    }

    private IEnumerator DealHealPerformer(HealGA healGA)
    {
        if (healGA.Targets_Player != null)
        {
            foreach (var target in healGA.Targets_Player)
            {
                target.TakeHeal(healGA.Amount);
                yield return new WaitForSeconds(0.15f);
            }
        }

        if (healGA.Targets_Enemy != null)
        {
            foreach (var target in healGA.Targets_Enemy)
            {
                target.TakeHeal(healGA.Amount);
                yield return new WaitForSeconds(0.15f);
            }
        }
    }

    private IEnumerator DealShieldPerformer(ShieldGA shieldGA)
    {
        if (shieldGA.Targets_Player != null)
        {
            foreach (var target in shieldGA.Targets_Player)
            {
                target.TakeShield(shieldGA.Actionner.GetComponent<PermanentView>(),null);
                yield return new WaitForSeconds(0.15f);
            }
        }

        if (shieldGA.Targets_Enemy != null)
        {
            foreach (var target in shieldGA.Targets_Enemy)
            {
                target.TakeShield(null,shieldGA.Actionner.GetComponent<EnemySlotView>());
                yield return new WaitForSeconds(0.15f);
            }
        }
    }

    private IEnumerator LoseShieldPerformer(LoseShieldGA loseShieldGA)
    {
        if (loseShieldGA.PermanentView != null)
        {
            foreach (PermanentView perm in loseShieldGA.PermanentView.PlayerShielded)
            {
                perm.RemoveShield(loseShieldGA.PermanentView,null);
                yield return new WaitForSeconds(0.10f);
            }
            foreach (EnemySlotView perm in loseShieldGA.PermanentView.EnemyShielded)
            {
                perm.RemoveShield(loseShieldGA.PermanentView,null);
                yield return new WaitForSeconds(0.10f);
            }
        }

        if (loseShieldGA.EnemySlotView != null)
        {
            foreach (PermanentView perm in loseShieldGA.EnemySlotView.PlayerShielded)
            {
                perm.RemoveShield(null,loseShieldGA.EnemySlotView);
                yield return new WaitForSeconds(0.10f);
            }
            foreach (EnemySlotView perm in loseShieldGA.EnemySlotView.EnemyShielded)
            {
                perm.RemoveShield(null,loseShieldGA.EnemySlotView);
                yield return new WaitForSeconds(0.10f);
            }
        }
    }
}
