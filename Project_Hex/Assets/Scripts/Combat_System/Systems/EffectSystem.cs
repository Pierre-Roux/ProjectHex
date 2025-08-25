using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class EffectSystem : Singleton<EffectSystem>
{
    public float AnimDelay = 0.25f;

    void OnEnable()
    {
        ActionSystem.AttachPerformer<DoEffectGA>(PerformEffectPerformer);
        ActionSystem.AttachPerformer<DealDamageGA>(DealDamagePerformer);
        ActionSystem.AttachPerformer<HealGA>(DealHealPerformer);
        ActionSystem.AttachPerformer<ShieldGA>(DealShieldPerformer);
        ActionSystem.AttachPerformer<LoseShieldGA>(LoseShieldPerformer);
        ActionSystem.AttachPerformer<DecountPlayerDecayGA>(DecountDecayPlayerPerformer);
        ActionSystem.AttachPerformer<DecountEnemyDecayGA>(DecountDecayEnemyPerformer);
        ActionSystem.AttachPerformer<AlterPowerGA>(AlterPowerPerformer);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<DoEffectGA>();
        ActionSystem.DetachPerformer<DealDamageGA>();
        ActionSystem.DetachPerformer<HealGA>();
        ActionSystem.DetachPerformer<ShieldGA>();
        ActionSystem.DetachPerformer<LoseShieldGA>();
        ActionSystem.DetachPerformer<DecountPlayerDecayGA>();
        ActionSystem.DetachPerformer<DecountEnemyDecayGA>();
        ActionSystem.DetachPerformer<AlterPowerGA>();
    }


    // Performers
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
                if (target.Shielded)
                {
                    if (target.PlayerShielder.Count != 0 && target.EnemyShielder.Count != 0)
                    {
                        var newtargetP = target.PlayerShielder[Random.Range(0, target.PlayerShielder.Count)];
                        var newtargetE = target.EnemyShielder[Random.Range(0, target.EnemyShielder.Count)];
                        if (Random.Range(0, 1) == 0)
                        {
                            newtargetP.TakeDamage(dealDamageGA.Amount);
                        }
                        else
                        {
                            newtargetE.TakeDamage(dealDamageGA.Amount);
                        }
                    }
                    else if (target.EnemyShielder.Count != 0)
                    {
                        var newtargetE = target.EnemyShielder[Random.Range(0, target.EnemyShielder.Count)];
                        newtargetE.TakeDamage(dealDamageGA.Amount);
                    }
                    else if (target.PlayerShielder.Count != 0)
                    {
                        var newtargetP = target.PlayerShielder[Random.Range(0, target.PlayerShielder.Count)];
                        newtargetP.TakeDamage(dealDamageGA.Amount);
                    }
                    yield return new WaitForSeconds(AnimDelay);
                }
                else
                {
                    target.TakeDamage(dealDamageGA.Amount);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }
        }

        if (dealDamageGA.Targets_Enemy != null)
        {
            foreach (var target in dealDamageGA.Targets_Enemy)
            {
                if (target.Shielded)
                {
                    if (target.PlayerShielder.Count != 0 && target.EnemyShielder.Count != 0)
                    {
                        var newtargetP = target.PlayerShielder[Random.Range(0, target.PlayerShielder.Count)];
                        var newtargetE = target.EnemyShielder[Random.Range(0, target.EnemyShielder.Count)];
                        if (Random.Range(0, 1) == 0)
                        {
                            newtargetP.TakeDamage(dealDamageGA.Amount);
                        }
                        else
                        {
                            newtargetE.TakeDamage(dealDamageGA.Amount);
                        }
                    }
                    else if (target.EnemyShielder.Count != 0)
                    {
                        var newtargetE = target.EnemyShielder[Random.Range(0, target.EnemyShielder.Count)];
                        newtargetE.TakeDamage(dealDamageGA.Amount);
                    }
                    else if (target.PlayerShielder.Count != 0)
                    {
                        var newtargetP = target.PlayerShielder[Random.Range(0, target.PlayerShielder.Count)];
                        newtargetP.TakeDamage(dealDamageGA.Amount);
                    }
                    yield return new WaitForSeconds(AnimDelay);
                }
                else
                {
                    target.TakeDamage(dealDamageGA.Amount);
                    yield return new WaitForSeconds(AnimDelay);
                }
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
                yield return new WaitForSeconds(AnimDelay);
            }
        }

        if (healGA.Targets_Enemy != null)
        {
            foreach (var target in healGA.Targets_Enemy)
            {
                target.TakeHeal(healGA.Amount);
                yield return new WaitForSeconds(AnimDelay);
            }
        }
    }

    private IEnumerator DealShieldPerformer(ShieldGA shieldGA)
    {
        if (shieldGA.Targets_Player != null)
        {
            foreach (var target in shieldGA.Targets_Player)
            {
                target.TakeShield(shieldGA.Actionner.GetComponent<PermanentView>(), null);
                yield return new WaitForSeconds(AnimDelay);
            }
        }

        if (shieldGA.Targets_Enemy != null)
        {
            foreach (var target in shieldGA.Targets_Enemy)
            {
                target.TakeShield(null, shieldGA.Actionner.GetComponent<EnemySlotView>());
                yield return new WaitForSeconds(AnimDelay);
            }
        }
    }

    private IEnumerator LoseShieldPerformer(LoseShieldGA loseShieldGA)
    {
        if (loseShieldGA.PermanentView != null)
        {
            foreach (PermanentView perm in loseShieldGA.PermanentView.PlayerShielded)
            {
                perm.RemoveShield(loseShieldGA.PermanentView, null);
                yield return new WaitForSeconds(AnimDelay);
            }
            foreach (EnemySlotView perm in loseShieldGA.PermanentView.EnemyShielded)
            {
                perm.RemoveShield(loseShieldGA.PermanentView, null);
                yield return new WaitForSeconds(AnimDelay);
            }
        }

        if (loseShieldGA.EnemySlotView != null)
        {
            foreach (PermanentView perm in loseShieldGA.EnemySlotView.PlayerShielded)
            {
                perm.RemoveShield(null, loseShieldGA.EnemySlotView);
                yield return new WaitForSeconds(AnimDelay);
            }
            foreach (EnemySlotView perm in loseShieldGA.EnemySlotView.EnemyShielded)
            {
                perm.RemoveShield(null, loseShieldGA.EnemySlotView);
                yield return new WaitForSeconds(AnimDelay);
            }
        }
    }

    private IEnumerator DecountDecayPlayerPerformer(DecountPlayerDecayGA decountPlayerDecayGA)
    {
        foreach (PermanentView permanentView in CombatSystem.Instance.Player_Permanents)
        {
            if (permanentView.DecayCounter > 0)
            {
                permanentView.DecayCounter--;
                if (permanentView.DecayCounter == 0)
                {
                    DiePermanentGA diepermanentGA = new(permanentView.IsCore, permanentView.Durability, permanentView.CardReferenceArchive, permanentView);
                    ActionSystem.Instance.AddReaction(diepermanentGA);
                }
            }
        }
        yield return null;
    }

    private IEnumerator DecountDecayEnemyPerformer(DecountEnemyDecayGA decountEnemyDecayGA)
    {
        foreach (EnemySlotView EnemySlot in CombatSystem.Instance.Enemy_Permanents)
        {
            if (EnemySlot.DecayCounter > 0)
            {
                EnemySlot.DecayCounter--;
                if (EnemySlot.DecayCounter == 0)
                {
                    DieEnemySlotGA dieEnemySlotGA = new(EnemySlot);
                    ActionSystem.Instance.AddReaction(dieEnemySlotGA);
                }
            }
        }
        yield return null;
    }

    private IEnumerator AlterPowerPerformer(AlterPowerGA alterPowerGA)
    {
        if (alterPowerGA.passive)
        {
            UnityEngine.Debug.Log("Passive on " + alterPowerGA.permaTypes);
            switch (alterPowerGA.permaTypes)
            {
                case PermaTypes.All:
                    CombatSystem.Instance.GeneralPower += alterPowerGA.Amount;
                    break;

                case PermaTypes.Players:
                    CombatSystem.Instance.PlayerGeneralPower += alterPowerGA.Amount;
                    UnityEngine.Debug.Log("PowerAugment by " + alterPowerGA.Amount);
                    break;

                case PermaTypes.Enemy:
                    CombatSystem.Instance.EnemyGeneralPower += alterPowerGA.Amount;
                    break;

                case PermaTypes.AllInvoc:
                    CombatSystem.Instance.Invoc_GeneralPower += alterPowerGA.Amount;
                    break;

                case PermaTypes.AllInvoc_Players:
                    CombatSystem.Instance.Invoc_PlayerGeneralPower += alterPowerGA.Amount;
                    break;

                case PermaTypes.AllInvoc_Enemy:
                    CombatSystem.Instance.Invoc_EnemyGeneralPower += alterPowerGA.Amount;
                    break;

                default:
                    break;
            }

            foreach (PermanentView item in CombatSystem.Instance.Player_Permanents)
            {
                // Update l'afichage pour les cartes
            }
            
            foreach (EnemySlotView item in CombatSystem.Instance.Enemy_Permanents)
            {
                item.UpdateIntentText(item.IntentAction);
            }
        }
        else
        {
            if (alterPowerGA.Targets_Player != null)
            {
                foreach (var target in alterPowerGA.Targets_Player)
                {
                    target.TakeAlterPower(alterPowerGA.Amount);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }

            if (alterPowerGA.Targets_Enemy != null)
            {
                foreach (var target in alterPowerGA.Targets_Enemy)
                {
                    target.TakeAlterPower(alterPowerGA.Amount);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }
        }
    }
}
