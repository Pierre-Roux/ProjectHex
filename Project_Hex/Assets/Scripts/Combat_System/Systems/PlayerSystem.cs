using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public class PlayerSystem : Singleton<PlayerSystem>
{
    void OnEnable()
    {
        ActionSystem.AttachPerformer<AttackEnemyGA>(AttackEnemyPerformer);
        ActionSystem.AttachPerformer<HealPlayerGA>(HealPlayerPerformer);
        ActionSystem.AttachPerformer<ShieldPlayerGA>(ShieldPlayerPerformer);
        ActionSystem.AttachPerformer<PlayerAlterPowerGA>(AlterPlayerPerformer);
        ActionSystem.AttachPerformer<PlayerLifeLossGA>(LifeLossPlayerPerformer);
        ActionSystem.AttachPerformer<PlayerGainLifeGA>(GainHPEnemyPerformer);

        ActionSystem.SubscribeReaction<AttackEnemyGA>(BeforeAttackPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<HealPlayerGA>(BeforeHealPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<ShieldPlayerGA>(BeforeShieldPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<PlayerAlterPowerGA>(BeforeAlterPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<PlayerLifeLossGA>(BeforeLifeLossPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<PlayerGainLifeGA>(BeforeGainHPPreReaction, ReactionTiming.PRE);

    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<AttackEnemyGA>();
        ActionSystem.DetachPerformer<HealPlayerGA>();
        ActionSystem.DetachPerformer<ShieldPlayerGA>();
        ActionSystem.DetachPerformer<PlayerAlterPowerGA>();
        ActionSystem.DetachPerformer<PlayerLifeLossGA>();
        ActionSystem.DetachPerformer<PlayerGainLifeGA>();

        ActionSystem.UnsubscribeReaction<AttackEnemyGA>(BeforeAttackPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<HealPlayerGA>(BeforeHealPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<ShieldPlayerGA>(BeforeShieldPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<PlayerAlterPowerGA>(BeforeAlterPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<PlayerLifeLossGA>(BeforeLifeLossPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<PlayerGainLifeGA>(BeforeGainHPPreReaction, ReactionTiming.PRE);
    }

    public int CalculateBonusPower(int BaseAmount, PermanentView permanentView)
    {
        int PassiveBonus = 0;

        if (permanentView.permaTypes.Contains(PermaTypes.Invoc))
        {
            PassiveBonus += CombatSystem.Instance.Invoc_PlayerGeneralPower + CombatSystem.Instance.Invoc_GeneralPower;
        }
        if (permanentView.permaTypes.Contains(PermaTypes.Decay))
        {
            PassiveBonus += CombatSystem.Instance.Decay_PlayerGeneralPower + CombatSystem.Instance.Decay_GeneralPower;
        }
        if (permanentView.permaTypes.Contains(PermaTypes.Hollow))
        {
            PassiveBonus += CombatSystem.Instance.Hollow_PlayerGeneralPower + CombatSystem.Instance.Hollow_GeneralPower;
        }
        if (permanentView.permaTypes.Contains(PermaTypes.Artillery))
        {
            PassiveBonus += CombatSystem.Instance.Artillery_PlayerGeneralPower + CombatSystem.Instance.Artillery_GeneralPower;
        }


        int finalDMG = 0;
        finalDMG = BaseAmount + permanentView.BonusPower + PassiveBonus + CombatSystem.Instance.PlayerGeneralPower + CombatSystem.Instance.GeneralPower; ;
        if (finalDMG < 0) finalDMG = 0;
        return finalDMG;
    }

    private IEnumerator AttackEnemyPerformer(AttackEnemyGA attackEnemyGA)
    {
        if (attackEnemyGA.Actionner != null)
        {
            PermanentView Attacker = attackEnemyGA.Actionner.GetComponent<PermanentView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            if (attackEnemyGA.playerTargets != null && attackEnemyGA.playerTargets.Count > 0)
            {
                int DamageAmount = CalculateBonusPower(attackEnemyGA.Damage, Attacker);

                ActionSystem.Instance.AddReaction(new DealDamageGA(DamageAmount, attackEnemyGA.DynamicAmount, attackEnemyGA.playerTargets, null));
            }

            if (attackEnemyGA.enemyTargets != null && attackEnemyGA.enemyTargets.Count > 0)
            {
                int DamageAmount = CalculateBonusPower(attackEnemyGA.Damage, Attacker);

                ActionSystem.Instance.AddReaction(new DealDamageGA(DamageAmount, attackEnemyGA.DynamicAmount, null, attackEnemyGA.enemyTargets));
            }
        }
        // dans le cas ou il n'y a pas de d'actionner c'est que c'est une attaque non directe mais du a un effet spécifique qui n'est pas cancel en cas de mort
        else
        {
            if (attackEnemyGA.playerTargets != null && attackEnemyGA.playerTargets.Count > 0)
            {
                ActionSystem.Instance.AddReaction(new DealDamageGA(attackEnemyGA.Damage, attackEnemyGA.DynamicAmount, attackEnemyGA.playerTargets, null));
            }

            if (attackEnemyGA.enemyTargets != null && attackEnemyGA.enemyTargets.Count > 0)
            {
                ActionSystem.Instance.AddReaction(new DealDamageGA(attackEnemyGA.Damage, attackEnemyGA.DynamicAmount, null, attackEnemyGA.enemyTargets));
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
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            if (healPlayerGA.playerTargets != null && healPlayerGA.playerTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new HealGA(healPlayerGA.HealAmount, healPlayerGA.DynamicAmount, healPlayerGA.playerTargets, null));

            if (healPlayerGA.enemyTargets != null && healPlayerGA.enemyTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new HealGA(healPlayerGA.HealAmount, healPlayerGA.DynamicAmount, null, healPlayerGA.enemyTargets));
        }
        // dans le cas ou il n'y a pas de d'actionner c'est que c'est une attaque non directe mais du a un effet spécifique qui n'est pas cancel en cas de mort
        else
        {
            if (healPlayerGA.playerTargets != null && healPlayerGA.playerTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new HealGA(healPlayerGA.HealAmount, healPlayerGA.DynamicAmount, healPlayerGA.playerTargets, null));

            if (healPlayerGA.enemyTargets != null && healPlayerGA.enemyTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new HealGA(healPlayerGA.HealAmount, healPlayerGA.DynamicAmount, null, healPlayerGA.enemyTargets));
        }
    }

    private IEnumerator ShieldPlayerPerformer(ShieldPlayerGA shieldPlayerGA)
    {
        if (shieldPlayerGA.Actionner != null)
        {
            PermanentView Attacker = shieldPlayerGA.Actionner.GetComponent<PermanentView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            if (shieldPlayerGA.playerTargets != null && shieldPlayerGA.playerTargets.Count > 0)
            {
                ShieldGA shieldGA = new ShieldGA(shieldPlayerGA.playerTargets, null);
                shieldGA.Actionner = shieldPlayerGA.Actionner;
                ActionSystem.Instance.AddReaction(shieldGA);
            }

            if (shieldPlayerGA.enemyTargets != null && shieldPlayerGA.enemyTargets.Count > 0)
            {
                ShieldGA shieldGA = new ShieldGA(null, shieldPlayerGA.enemyTargets);
                shieldGA.Actionner = shieldPlayerGA.Actionner;
                ActionSystem.Instance.AddReaction(shieldGA);
            }
        }
    }

    private IEnumerator AlterPlayerPerformer(PlayerAlterPowerGA playerAlterPowerGA)
    {
        if (playerAlterPowerGA.Actionner != null)
        {
            PermanentView Attacker = playerAlterPowerGA.Actionner.GetComponent<PermanentView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);
            if (playerAlterPowerGA.passive)
            {
                ActionSystem.Instance.AddReaction(new AlterPowerGA(playerAlterPowerGA.Amount, playerAlterPowerGA.DynamicAmount, playerAlterPowerGA.passive, playerAlterPowerGA.permaTypes, null, null, playerAlterPowerGA.targetMode));
            }
            else
            {
                if (playerAlterPowerGA.playerTargets != null && playerAlterPowerGA.playerTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new AlterPowerGA(playerAlterPowerGA.Amount, playerAlterPowerGA.DynamicAmount, playerAlterPowerGA.passive, playerAlterPowerGA.permaTypes, playerAlterPowerGA.playerTargets, null));

                if (playerAlterPowerGA.enemyTargets != null && playerAlterPowerGA.enemyTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new AlterPowerGA(playerAlterPowerGA.Amount, playerAlterPowerGA.DynamicAmount, playerAlterPowerGA.passive, playerAlterPowerGA.permaTypes, null, playerAlterPowerGA.enemyTargets));
            }
        }
        // dans le cas ou il n'y a pas de d'actionner c'est que c'est une attaque non directe mais du a un effet spécifique qui n'est pas cancel en cas de mort
        else
        {
            if (playerAlterPowerGA.passive)
            {
                ActionSystem.Instance.AddReaction(new AlterPowerGA(playerAlterPowerGA.Amount, playerAlterPowerGA.DynamicAmount, playerAlterPowerGA.passive, playerAlterPowerGA.permaTypes, null, null, playerAlterPowerGA.targetMode));
            }
            else
            {
                if (playerAlterPowerGA.playerTargets != null && playerAlterPowerGA.playerTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new AlterPowerGA(playerAlterPowerGA.Amount, playerAlterPowerGA.DynamicAmount, playerAlterPowerGA.passive, playerAlterPowerGA.permaTypes, playerAlterPowerGA.playerTargets, null));

                if (playerAlterPowerGA.enemyTargets != null && playerAlterPowerGA.enemyTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new AlterPowerGA(playerAlterPowerGA.Amount, playerAlterPowerGA.DynamicAmount, playerAlterPowerGA.passive, playerAlterPowerGA.permaTypes, null, playerAlterPowerGA.enemyTargets));
            }
        }
    }

    private IEnumerator LifeLossPlayerPerformer(PlayerLifeLossGA playerLifeLossGA)
    {
        if (playerLifeLossGA.Actionner != null)
        {
            PermanentView Attacker = playerLifeLossGA.Actionner.GetComponent<PermanentView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            if (playerLifeLossGA.playerTargets != null && playerLifeLossGA.playerTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new LifeLossGA(playerLifeLossGA.Amount, playerLifeLossGA.DynamicAmount, playerLifeLossGA.playerTargets, null));

            if (playerLifeLossGA.enemyTargets != null && playerLifeLossGA.enemyTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new LifeLossGA(playerLifeLossGA.Amount, playerLifeLossGA.DynamicAmount, null, playerLifeLossGA.enemyTargets));
        }
        // dans le cas ou il n'y a pas de d'actionner c'est que c'est une attaque non directe mais du a un effet spécifique qui n'est pas cancel en cas de mort
        else
        {
            if (playerLifeLossGA.playerTargets != null && playerLifeLossGA.playerTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new LifeLossGA(playerLifeLossGA.Amount, playerLifeLossGA.DynamicAmount, playerLifeLossGA.playerTargets, null));

            if (playerLifeLossGA.enemyTargets != null && playerLifeLossGA.enemyTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new LifeLossGA(playerLifeLossGA.Amount, playerLifeLossGA.DynamicAmount, null, playerLifeLossGA.enemyTargets));
        }
    }

    private IEnumerator GainHPEnemyPerformer(PlayerGainLifeGA playerGainLifeGA)
    {
        if (playerGainLifeGA.Actionner != null)
        {
            PermanentView Attacker = playerGainLifeGA.Actionner.GetComponent<PermanentView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);
            if (playerGainLifeGA.passive)
            {
                ActionSystem.Instance.AddReaction(new GainLifeGA(playerGainLifeGA.Amount, playerGainLifeGA.DynamicAmount, playerGainLifeGA.passive, playerGainLifeGA.permaTypes, null, null, playerGainLifeGA.targetMode));
            }
            else
            {
                if (playerGainLifeGA.playerTargets != null && playerGainLifeGA.playerTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new GainLifeGA(playerGainLifeGA.Amount, playerGainLifeGA.DynamicAmount, playerGainLifeGA.passive, playerGainLifeGA.permaTypes, playerGainLifeGA.playerTargets, null));

                if (playerGainLifeGA.enemyTargets != null && playerGainLifeGA.enemyTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new GainLifeGA(playerGainLifeGA.Amount, playerGainLifeGA.DynamicAmount, playerGainLifeGA.passive, playerGainLifeGA.permaTypes, null, playerGainLifeGA.enemyTargets));
            }
        }
        // dans le cas ou il n'y a pas de d'actionner c'est que c'est une attaque non directe mais du a un effet spécifique qui n'est pas cancel en cas de mort
        else
        {
            if (playerGainLifeGA.passive)
            {
                ActionSystem.Instance.AddReaction(new GainLifeGA(playerGainLifeGA.Amount, playerGainLifeGA.DynamicAmount, playerGainLifeGA.passive, playerGainLifeGA.permaTypes, null, null, playerGainLifeGA.targetMode));
            }
            else
            {
                if (playerGainLifeGA.playerTargets != null && playerGainLifeGA.playerTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new GainLifeGA(playerGainLifeGA.Amount, playerGainLifeGA.DynamicAmount, playerGainLifeGA.passive, playerGainLifeGA.permaTypes, playerGainLifeGA.playerTargets, null));

                if (playerGainLifeGA.enemyTargets != null && playerGainLifeGA.enemyTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new GainLifeGA(playerGainLifeGA.Amount, playerGainLifeGA.DynamicAmount, playerGainLifeGA.passive, playerGainLifeGA.permaTypes, null, playerGainLifeGA.enemyTargets));
            }
        }
    }

    private void BeforeAttackPreReaction(AttackEnemyGA attackEnemyGA)
    {
        if (attackEnemyGA.Actionner != null)
        {
            PermanentView Attacker = attackEnemyGA.Actionner.GetComponent<PermanentView>();
            Attacker.SetPosition(Attacker.transform.position);            
        }
    }

    private void BeforeHealPreReaction(HealPlayerGA healPlayerGA)
    {
        if (healPlayerGA.Actionner != null)
        {
            PermanentView Attacker = healPlayerGA.Actionner.GetComponent<PermanentView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeShieldPreReaction(ShieldPlayerGA shieldPlayerGA)
    {
        if (shieldPlayerGA.Actionner != null)
        {
            PermanentView Attacker = shieldPlayerGA.Actionner.GetComponent<PermanentView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeAlterPreReaction(PlayerAlterPowerGA playerAlterPowerGA)
    {
        if (playerAlterPowerGA.Actionner != null)
        {
            PermanentView Attacker = playerAlterPowerGA.Actionner.GetComponent<PermanentView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeLifeLossPreReaction(PlayerLifeLossGA playerLifeLossGA)
    {
        if (playerLifeLossGA.Actionner != null)
        {
            PermanentView Attacker = playerLifeLossGA.Actionner.GetComponent<PermanentView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }
    
    private void BeforeGainHPPreReaction(PlayerGainLifeGA playerGainLifeGA)
    {
        if (playerGainLifeGA.Actionner != null)
        {
            PermanentView Attacker = playerGainLifeGA.Actionner.GetComponent<PermanentView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }
}
