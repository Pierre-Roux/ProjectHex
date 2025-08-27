using System.Collections;
using System.Diagnostics;
using Unity.VisualScripting;
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
        ActionSystem.AttachPerformer<LifeLossGA>(LifeLossPerformer);
        ActionSystem.AttachPerformer<DiscardCardGA>(DiscardCardPerformer);
        ActionSystem.AttachPerformer<GainLifeGA>(GainLifePerformer);
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
        ActionSystem.DetachPerformer<LifeLossGA>();
        ActionSystem.DetachPerformer<DiscardCardGA>();
        ActionSystem.DetachPerformer<GainLifeGA>();
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
            UnityEngine.Debug.Log("Passive on " + alterPowerGA.permaTypes + " of " + alterPowerGA.targetMode);
            switch (alterPowerGA.targetMode)
            {
                case TargetMode.All_All:
                    switch (alterPowerGA.permaTypes)
                    {
                        case PermaTypes.Artillery:
                            CombatSystem.Instance.Artillery_GeneralPower += alterPowerGA.Amount;
                            break;
                        case PermaTypes.Decay:
                            CombatSystem.Instance.Decay_GeneralPower += alterPowerGA.Amount;
                            break;
                        case PermaTypes.Hollow:
                            CombatSystem.Instance.Hollow_GeneralPower += alterPowerGA.Amount;
                            break;
                        case PermaTypes.Invoc:
                            CombatSystem.Instance.Invoc_GeneralPower += alterPowerGA.Amount;
                            break;
                        default:
                            CombatSystem.Instance.GeneralPower += alterPowerGA.Amount;
                            break;
                    }
                    break;

                case TargetMode.All_Player:
                    switch (alterPowerGA.permaTypes)
                    {
                        case PermaTypes.Artillery:
                            CombatSystem.Instance.Artillery_PlayerGeneralPower += alterPowerGA.Amount;
                            break;
                        case PermaTypes.Decay:
                            CombatSystem.Instance.Decay_PlayerGeneralPower += alterPowerGA.Amount;
                            break;
                        case PermaTypes.Hollow:
                            CombatSystem.Instance.Hollow_PlayerGeneralPower += alterPowerGA.Amount;
                            break;
                        case PermaTypes.Invoc:
                            CombatSystem.Instance.Invoc_PlayerGeneralPower += alterPowerGA.Amount;
                            break;
                        default:
                            CombatSystem.Instance.PlayerGeneralPower += alterPowerGA.Amount;
                            break;
                    }
                    break;

                case TargetMode.All_Enemy:
                    switch (alterPowerGA.permaTypes)
                    {
                        case PermaTypes.Artillery:
                            CombatSystem.Instance.Artillery_EnemyGeneralPower += alterPowerGA.Amount;
                            break;
                        case PermaTypes.Decay:
                            CombatSystem.Instance.Decay_EnemyGeneralPower += alterPowerGA.Amount;
                            break;
                        case PermaTypes.Hollow:
                            CombatSystem.Instance.Hollow_EnemyGeneralPower += alterPowerGA.Amount;
                            break;
                        case PermaTypes.Invoc:
                            CombatSystem.Instance.Invoc_EnemyGeneralPower += alterPowerGA.Amount;
                            break;
                        default:
                            CombatSystem.Instance.EnemyGeneralPower += alterPowerGA.Amount;
                            break;
                    }
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

    private IEnumerator LifeLossPerformer(LifeLossGA lifeLossGA)
    {
        if (lifeLossGA.Targets_Player != null)
        {
            foreach (var target in lifeLossGA.Targets_Player)
            {
                target.TakeLifeLoss(lifeLossGA.Amount);
                yield return new WaitForSeconds(AnimDelay);
            }
        }

        if (lifeLossGA.Targets_Enemy != null)
        {
            foreach (var target in lifeLossGA.Targets_Enemy)
            {
                target.TakeLifeLoss(lifeLossGA.Amount);
                yield return new WaitForSeconds(AnimDelay);
            }
        }
    }

    private IEnumerator DiscardCardPerformer(DiscardCardGA discardCardGA)
    {
        foreach (CardView item in discardCardGA.CardViews)
        {
            CardSystem.Instance.handView.RemoveCard(item.Card);
            CardSystem.Instance.hand.Remove(item.Card);  
            StartCoroutine(CardSystem.Instance.DiscardCard(item, true));
            yield return null;
        }
    }
    
    private IEnumerator GainLifePerformer(GainLifeGA gainLifeGA)
    {
        if (gainLifeGA.passive)
        {
            UnityEngine.Debug.Log("Passive on " + gainLifeGA.permaTypes + " of " + gainLifeGA.targetMode);
            switch (gainLifeGA.targetMode)
            {
                case TargetMode.All_All:
                    switch (gainLifeGA.permaTypes)
                    {
                        case PermaTypes.Artillery:
                            CombatSystem.Instance.Artillery_GeneralHPGain += gainLifeGA.Amount;
                            break;
                        case PermaTypes.Decay:
                            CombatSystem.Instance.Decay_GeneralHPGain += gainLifeGA.Amount;
                            break;
                        case PermaTypes.Hollow:
                            CombatSystem.Instance.Hollow_GeneralHPGain += gainLifeGA.Amount;
                            break;
                        case PermaTypes.Invoc:
                            CombatSystem.Instance.Invoc_GeneralHPGain += gainLifeGA.Amount;
                            break;
                        default:
                            CombatSystem.Instance.GeneralHPGain += gainLifeGA.Amount;
                            break;
                    }
                    break;

                case TargetMode.All_Player:
                    switch (gainLifeGA.permaTypes)
                    {
                        case PermaTypes.Artillery:
                            CombatSystem.Instance.Artillery_PlayerGeneralHPGain += gainLifeGA.Amount;
                            break;
                        case PermaTypes.Decay:
                            CombatSystem.Instance.Decay_PlayerGeneralHPGain += gainLifeGA.Amount;
                            break;
                        case PermaTypes.Hollow:
                            CombatSystem.Instance.Hollow_PlayerGeneralHPGain += gainLifeGA.Amount;
                            break;
                        case PermaTypes.Invoc:
                            CombatSystem.Instance.Invoc_PlayerGeneralHPGain += gainLifeGA.Amount;
                            break;
                        default:
                            CombatSystem.Instance.PlayerGeneralHPGain += gainLifeGA.Amount;
                            break;
                    }
                    break;

                case TargetMode.All_Enemy:
                    switch (gainLifeGA.permaTypes)
                    {
                        case PermaTypes.Artillery:
                            CombatSystem.Instance.Artillery_EnemyGeneralHPGain += gainLifeGA.Amount;
                            break;
                        case PermaTypes.Decay:
                            CombatSystem.Instance.Decay_EnemyGeneralHPGain += gainLifeGA.Amount;
                            break;
                        case PermaTypes.Hollow:
                            CombatSystem.Instance.Hollow_EnemyGeneralHPGain += gainLifeGA.Amount;
                            break;
                        case PermaTypes.Invoc:
                            CombatSystem.Instance.Invoc_EnemyGeneralHPGain += gainLifeGA.Amount;
                            break;
                        default:
                            CombatSystem.Instance.EnemyGeneralHPGain += gainLifeGA.Amount;
                            break;
                    }
                    break;
                default:
                    break;
            }

            foreach (PermanentView item in CombatSystem.Instance.Player_Permanents)
            {
                item.UpdateLife();
            }

            foreach (EnemySlotView item in CombatSystem.Instance.Enemy_Permanents)
            {
                item.UpdateLife();
            }
        }
        else
        {
            if (gainLifeGA.Targets_Player != null)
            {
                foreach (var target in gainLifeGA.Targets_Player)
                {
                    target.GainLife(gainLifeGA.Amount);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }

            if (gainLifeGA.Targets_Enemy != null)
            {
                foreach (var target in gainLifeGA.Targets_Enemy)
                {
                    target.GainLife(gainLifeGA.Amount);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }
        }
    }
}
