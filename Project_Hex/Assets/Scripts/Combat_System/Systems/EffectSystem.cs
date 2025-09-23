using System.Collections;
using System.Collections.Generic;
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
        ActionSystem.AttachPerformer<ScryGA>(ScryPerformer);
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
        ActionSystem.DetachPerformer<ScryGA>();
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
        if (dealDamageGA.DynamicAmount != DynamicAmount.NULL)
        {
            if (dealDamageGA.Actionner == null)
            {
                dealDamageGA.Amount = TargetSystem.Instance.GetDynamicAmount(dealDamageGA.DynamicAmount, null, null);
            }
            else if (dealDamageGA.Actionner.GetComponent<PermanentView>() != null)
            {
                dealDamageGA.Amount = TargetSystem.Instance.GetDynamicAmount(dealDamageGA.DynamicAmount, dealDamageGA.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                dealDamageGA.Amount = TargetSystem.Instance.GetDynamicAmount(dealDamageGA.DynamicAmount, null, dealDamageGA.Actionner.GetComponent<EnemySlotView>());
            }
        }
        if (dealDamageGA.playerTargets != null)
        {
            foreach (var target in dealDamageGA.playerTargets)
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

        if (dealDamageGA.enemyTargets != null)
        {
            foreach (var target in dealDamageGA.enemyTargets)
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
        if (healGA.DynamicAmount != DynamicAmount.NULL)
        {
            if (healGA.Actionner == null)
            {
                healGA.Amount = TargetSystem.Instance.GetDynamicAmount(healGA.DynamicAmount,null,null);
            }
            else if (healGA.Actionner.GetComponent<PermanentView>() != null)
            {
                healGA.Amount = TargetSystem.Instance.GetDynamicAmount(healGA.DynamicAmount,healGA.Actionner.GetComponent<PermanentView>(),null);
            }
            else
            {
                healGA.Amount = TargetSystem.Instance.GetDynamicAmount(healGA.DynamicAmount,null,healGA.Actionner.GetComponent<EnemySlotView>());
            }    
        }

        if (healGA.playerTargets != null)
        {
            foreach (var target in healGA.playerTargets)
            {
                target.TakeHeal(healGA.Amount);
                yield return new WaitForSeconds(AnimDelay);
            }
        }

        if (healGA.enemyTargets != null)
        {
            foreach (var target in healGA.enemyTargets)
            {
                target.TakeHeal(healGA.Amount);
                yield return new WaitForSeconds(AnimDelay);
            }
        }
    }

    private IEnumerator DealShieldPerformer(ShieldGA shieldGA)
    {
        if (shieldGA.playerTargets != null)
        {
            foreach (var target in shieldGA.playerTargets)
            {
                target.TakeShield(shieldGA.Actionner.GetComponent<PermanentView>(), null);
                yield return new WaitForSeconds(AnimDelay);
            }
        }

        if (shieldGA.enemyTargets != null)
        {
            foreach (var target in shieldGA.enemyTargets)
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
        if (alterPowerGA.DynamicAmount != DynamicAmount.NULL)
        {
            if (alterPowerGA.Actionner == null)
            {
                alterPowerGA.Amount = TargetSystem.Instance.GetDynamicAmount(alterPowerGA.DynamicAmount,null,null);
            }
            else if (alterPowerGA.Actionner.GetComponent<PermanentView>() != null)
            {
                alterPowerGA.Amount = TargetSystem.Instance.GetDynamicAmount(alterPowerGA.DynamicAmount,alterPowerGA.Actionner.GetComponent<PermanentView>(),null);
            }
            else
            {
                alterPowerGA.Amount = TargetSystem.Instance.GetDynamicAmount(alterPowerGA.DynamicAmount,null,alterPowerGA.Actionner.GetComponent<EnemySlotView>());
            }    
        }
        if (alterPowerGA.passive)
        {
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
            if (alterPowerGA.playerTargets != null)
            {
                foreach (var target in alterPowerGA.playerTargets)
                {
                    target.TakeAlterPower(alterPowerGA.Amount);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }

            if (alterPowerGA.enemyTargets != null)
            {
                foreach (var target in alterPowerGA.enemyTargets)
                {
                    target.TakeAlterPower(alterPowerGA.Amount);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }
        }
    }

    private IEnumerator LifeLossPerformer(LifeLossGA lifeLossGA)
    {
        if (lifeLossGA.DynamicAmount != DynamicAmount.NULL)
        {
            if (lifeLossGA.Actionner == null)
            {
                lifeLossGA.Amount = TargetSystem.Instance.GetDynamicAmount(lifeLossGA.DynamicAmount,null,null);
            }
            else if (lifeLossGA.Actionner.GetComponent<PermanentView>() != null)
            {
                lifeLossGA.Amount = TargetSystem.Instance.GetDynamicAmount(lifeLossGA.DynamicAmount,lifeLossGA.Actionner.GetComponent<PermanentView>(),null);
            }
            else
            {
                lifeLossGA.Amount = TargetSystem.Instance.GetDynamicAmount(lifeLossGA.DynamicAmount,null,lifeLossGA.Actionner.GetComponent<EnemySlotView>());
            }    
        }
        if (lifeLossGA.playerTargets != null)
        {
            foreach (var target in lifeLossGA.playerTargets)
            {
                target.TakeLifeLoss(lifeLossGA.Amount);
                yield return new WaitForSeconds(AnimDelay);
            }
        }

        if (lifeLossGA.enemyTargets != null)
        {
            foreach (var target in lifeLossGA.enemyTargets)
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

    private IEnumerator ScryPerformer(ScryGA scryGA)
    {
        if (scryGA.DynamicAmount != DynamicAmount.NULL)
        {
            scryGA.Amount = TargetSystem.Instance.GetDynamicAmount(scryGA.DynamicAmount);
        }
        List<Card> topCards = CardSystem.Instance.drawPile.TakeTop(scryGA.Amount);
        if (topCards.Count == 0) yield break;

        CardSystem.Instance.ShowScryPanel(topCards);

        CardSystem.Instance.ScryScrollRect.enabled = false;

        yield return new WaitUntil(() => CardSystem.Instance.ScryCardViews.Count == 0);

        CardSystem.Instance.HideScryPanel();
        yield return null;
    }
    
    private IEnumerator GainLifePerformer(GainLifeGA gainLifeGA)
    {
        if (gainLifeGA.DynamicAmount != DynamicAmount.NULL)
        {
            if (gainLifeGA.Actionner == null)
            {
                gainLifeGA.Amount = TargetSystem.Instance.GetDynamicAmount(gainLifeGA.DynamicAmount,null,null);
            }
            else if (gainLifeGA.Actionner.GetComponent<PermanentView>() != null)
            {
                gainLifeGA.Amount = TargetSystem.Instance.GetDynamicAmount(gainLifeGA.DynamicAmount,gainLifeGA.Actionner.GetComponent<PermanentView>(),null);
            }
            else
            {
                gainLifeGA.Amount = TargetSystem.Instance.GetDynamicAmount(gainLifeGA.DynamicAmount,null,gainLifeGA.Actionner.GetComponent<EnemySlotView>());
            }    
        }
        if (gainLifeGA.passive)
        {
            //UnityEngine.Debug.Log("Passive on " + gainLifeGA.permaTypes + " of " + gainLifeGA.targetMode);
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
            if (gainLifeGA.playerTargets != null)
            {
                foreach (var target in gainLifeGA.playerTargets)
                {
                    target.GainLife(gainLifeGA.Amount);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }

            if (gainLifeGA.enemyTargets != null)
            {
                foreach (var target in gainLifeGA.enemyTargets)
                {
                    target.GainLife(gainLifeGA.Amount);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }
        }
    }
}
