using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using FMODUnity;

public class PermanentView : MonoBehaviour
{
    [SerializeField] SpriteRenderer PermanentSpriteRenderer;
    [SerializeField] TMP_Text HealthText;
    [SerializeField] public GameObject ShieldVisual;
    [SerializeField] public bool UnShieldable;

    [SerializeField] public EventReference DieSound;
    [SerializeField] public EventReference HollowDieSound;
    [SerializeField] public EventReference BeingDamageSound;
    [SerializeField] public EventReference BeingHealSound;
    [SerializeField] public EventReference BeingShieldSound;
    [SerializeField] public EventReference LoseShieldSound;
    [SerializeField] public EventReference GainPowerSound;
    [SerializeField] public EventReference LosePowerSound;
    [SerializeField] public EventReference TakeLifeLossSound;
    [SerializeField] public EventReference BuffLifeSound;
    [SerializeField] public EventReference DebuffLifeSound;
    [SerializeField] public EventReference SelectedSound;
    [SerializeField] public EventReference UnSelectedSound;

    [HideInInspector] public bool IsCore { get; set; }
    [HideInInspector] private int MaxLife { get; set; }
    [HideInInspector] public int currentLife { get; set; }
    [HideInInspector] public int baseLife { get; set; }
    [HideInInspector] public int MaxDurability { get; set; }
    [HideInInspector] public int Durability { get; set; }
    [HideInInspector] public int DecayCounter { get; set; }
    [HideInInspector] public int BonusPower { get; set; }
    [HideInInspector] public int CurrentHPBonus { get; set; }
    [HideInInspector] public Card CardReferenceArchive;
    [HideInInspector] public bool IsDead = false;
    [HideInInspector] public Vector3 InitialPosition { get; set; }
    [HideInInspector] public PermanentArea permanentArea;

    [HideInInspector] public List<PermanentView> PlayerShielder;
    [HideInInspector] public List<EnemySlotView> EnemyShielder;
    [HideInInspector] public List<PermanentView> PlayerShielded;
    [HideInInspector] public List<EnemySlotView> EnemyShielded;
    [HideInInspector] public bool Targetable = true;
    [HideInInspector] public bool Shielded;
    [HideInInspector] public bool Activated;

    [HideInInspector] public List<PermaTypes> permaTypes = new List<PermaTypes>();

    public void Setup(Card cardReference)
    {
        Targetable = true;
        IsCore = false;
        CardReferenceArchive = cardReference;
        PermanentSpriteRenderer.sprite = cardReference.data.PermanentImage;
        baseLife = cardReference.data.life;
        MaxLife = CalculateBonusLife(baseLife);
        currentLife = MaxLife;
        MaxDurability = cardReference.MaxDurability;
        Durability = cardReference.Durability;
        permanentArea = cardReference.data.permanentArea;
        UnShieldable = cardReference.UnShieldable;
        DecayCounter = cardReference.DecayCounter;

        // Gère les types
        permaTypes.Clear();
        if (cardReference.data.isInvoc) permaTypes.Add(PermaTypes.Invoc);
        if (DecayCounter > 0) permaTypes.Add(PermaTypes.Decay);
        if (cardReference.MaxDurability > 0 && cardReference.Durability == 0) permaTypes.Add(PermaTypes.Hollow);
        if (cardReference.data.isArtillery) permaTypes.Add(PermaTypes.Artillery);

        ShieldVisual.SetActive(false);
        UpdateLifeText();

        // affichage graphique du hollow
        if (permaTypes.Contains(PermaTypes.Hollow))
        {
            UpdateHollowVisual();
        }

        //Audio
        if (cardReference.DieSound.Path != "") DieSound = cardReference.DieSound;
        if (cardReference.HollowDieSound.Path != "") HollowDieSound = cardReference.HollowDieSound;
        if (cardReference.BeingDamageSound.Path != "") BeingDamageSound = cardReference.BeingDamageSound;
        if (cardReference.BeingHealSound.Path != "") BeingHealSound = cardReference.BeingHealSound;
        if (cardReference.BeingShieldSound.Path != "") BeingShieldSound = cardReference.BeingShieldSound;
        if (cardReference.LoseShieldSound.Path != "") LoseShieldSound = cardReference.LoseShieldSound;
        if (cardReference.GainPowerSound.Path != "") GainPowerSound = cardReference.GainPowerSound;
        if (cardReference.LosePowerSound.Path != "") LosePowerSound = cardReference.LosePowerSound;
        if (cardReference.TakeLifeLossSound.Path != "") TakeLifeLossSound = cardReference.TakeLifeLossSound;
        if (cardReference.BuffLifeSound.Path != "") BuffLifeSound = cardReference.BuffLifeSound;
        if (cardReference.DebuffLifeSound.Path != "") DebuffLifeSound = cardReference.DebuffLifeSound;
        if (cardReference.SelectedSound.Path != "") SelectedSound = cardReference.SelectedSound;
        if (cardReference.UnSelectedSound.Path != "") UnSelectedSound = cardReference.UnSelectedSound;
    }

    public void SetPosition(Vector3 pos)
    {
        InitialPosition = pos;
    }

    public void SetupCore(PlayerData CoreData)
    {
        Targetable = true;
        IsCore = true;
        PermanentSpriteRenderer.sprite = CoreData.CoreImage;
        permanentArea = PermanentArea.none;
        baseLife = CoreData.CoreHealth;
        MaxLife = CalculateBonusLife(baseLife);
        currentLife = MaxLife; 
        UnShieldable = false;
        ShieldVisual.SetActive(false);
        UpdateLifeText();
    }

    public void UpdateLifeText()
    {
        HealthText.text = currentLife.ToString();
    }

    public void UpdateHollowVisual()
    {
        Color c = PermanentSpriteRenderer.color;
        c.a = 0.5f;
        PermanentSpriteRenderer.color = c;        
    }
    
    public int CalculateBonusPower(int baseAmount)
    {
        int passiveBonus = 0;

        if (permaTypes.Contains(PermaTypes.Invoc))
            passiveBonus += CombatSystem.Instance.Invoc_PlayerGeneralHPGain + CombatSystem.Instance.Invoc_GeneralPower;
        if (permaTypes.Contains(PermaTypes.Decay))
            passiveBonus += CombatSystem.Instance.Decay_PlayerGeneralHPGain + CombatSystem.Instance.Decay_GeneralPower;
        if (permaTypes.Contains(PermaTypes.Hollow))
            passiveBonus += CombatSystem.Instance.Hollow_PlayerGeneralHPGain + CombatSystem.Instance.Hollow_GeneralPower;
        if (permaTypes.Contains(PermaTypes.Artillery))
            passiveBonus += CombatSystem.Instance.Artillery_PlayerGeneralHPGain + CombatSystem.Instance.Artillery_GeneralPower;

        int finalDMG = baseAmount + BonusPower + passiveBonus + CombatSystem.Instance.EnemyGeneralPower + CombatSystem.Instance.GeneralPower;
        return Mathf.Max(0, finalDMG);
    }
    public int CalculateBonusLife(int baseAmount)
    {
        int passiveBonus = 0;

        if (permaTypes.Contains(PermaTypes.Invoc))
            passiveBonus += CombatSystem.Instance.Invoc_PlayerGeneralHPGain + CombatSystem.Instance.Invoc_GeneralHPGain;
        if (permaTypes.Contains(PermaTypes.Decay))
            passiveBonus += CombatSystem.Instance.Decay_PlayerGeneralHPGain + CombatSystem.Instance.Decay_GeneralHPGain;
        if (permaTypes.Contains(PermaTypes.Hollow))
            passiveBonus += CombatSystem.Instance.Hollow_PlayerGeneralHPGain + CombatSystem.Instance.Hollow_GeneralHPGain;
        if (permaTypes.Contains(PermaTypes.Artillery))
            passiveBonus += CombatSystem.Instance.Artillery_PlayerGeneralHPGain + CombatSystem.Instance.Artillery_GeneralHPGain;

        int finalHP = baseAmount + passiveBonus + CombatSystem.Instance.PlayerGeneralHPGain + CombatSystem.Instance.GeneralHPGain;
        return Mathf.Max(0, finalHP);
    }

    public void UpdateLife()
    {
        int passiveBonus = CalculateBonusLife(0);
        MaxLife = baseLife + passiveBonus;

        if (currentLife > MaxLife)
        {
            currentLife = MaxLife;
        }
        else
        {
            if (currentLife + passiveBonus > MaxLife)
            {
                currentLife = MaxLife;
            }
            else
            {
                currentLife = currentLife + passiveBonus;
            }
        }


        UpdateLifeText();
    }

    public void TakeDamage(int Amount, Card CardActionner = null, GameObject Actionner = null)
    {
        if (Amount <= 0) return;
        currentLife -= Amount;
        UpdateLifeText();

        if (!IsDead)
        {
            transform.DOShakePosition(0.2f, 0.5f);
            TriggerEventGA triggerPermanentEventGA = new(Events.OnDamaged,null,this,null);
            ActionSystem.Instance.AddReaction(triggerPermanentEventGA);
        }

        if (currentLife <= 0)
        {
            if (!IsDead)
            {
                DiePermanentGA diePermanentGA = new(IsCore, Durability, CardReferenceArchive, this);
                ActionSystem.Instance.AddReaction(diePermanentGA);
                OnKillTrigger(CardActionner, Actionner);
                IsDead = true;
            }
        }
        else
        {
            RuntimeManager.PlayOneShot(BeingDamageSound);
        }
    }

    public void OnKillTrigger(Card CardActionner, GameObject Actionner)
    {
        if (Actionner != null)
        {
            if (Actionner.GetComponent<PermanentView>() != null)
            {
                TriggerEventGA triggerEventGA = new(Events.OnKill, null, Actionner.GetComponent<PermanentView>(), null);
                ActionSystem.Instance.AddReaction(triggerEventGA);
            }
            else if (Actionner.GetComponent<EnemySlotView>())
            {
                TriggerEventGA triggerEventGA = new(Events.OnKill, null, null, Actionner.GetComponent<EnemySlotView>());
                ActionSystem.Instance.AddReaction(triggerEventGA);
            }
        }
        else if (CardActionner != null)
        {
            TriggerEventGA triggerEventGA = new(Events.OnKill, CardActionner, null, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }
    }

    public void TakeHeal(int Amount)
    {
        currentLife += Amount;
        if (currentLife > MaxLife)
        {
            currentLife = MaxLife;
        }
        RuntimeManager.PlayOneShot(BeingHealSound);
        transform.DOShakePosition(0.1f, 0.1f);
        UpdateLifeText();
    }

    public void TakeShield(PermanentView playerShielder = null, EnemySlotView enemyShielder = null)
    {
        if (!UnShieldable)
        {
            RuntimeManager.PlayOneShot(BeingShieldSound);
            if (playerShielder != null)
            {
                if (!PlayerShielder.Contains(playerShielder))
                {
                    PlayerShielder.Add(playerShielder);
                    playerShielder.GetComponent<PermanentView>().PlayerShielded.Add(this);
                }
            }

            if (enemyShielder != null)
            {
                if (!EnemyShielder.Contains(enemyShielder))
                {
                    EnemyShielder.Add(enemyShielder);
                    enemyShielder.GetComponent<EnemySlotView>().PlayerShielded.Add(this);
                }
            }
            UpdateShield();
        }
    }

    public void RemoveShield(PermanentView playerShielder = null, EnemySlotView enemyShielder = null)
    {
        if (playerShielder != null)
        {
            PlayerShielder.Remove(playerShielder);
        }
        if (enemyShielder != null)
        {
            EnemyShielder.Remove(enemyShielder);
        }
        UpdateShield();        
    }

    public void UpdateShield()
    {
        if (PlayerShielder.Count != 0 || EnemyShielder.Count != 0)
        {
            ShieldVisual.SetActive(true);
            Shielded = true;
        }
        else
        {
            RuntimeManager.PlayOneShot(LoseShieldSound);
            ShieldVisual.SetActive(false);
            Shielded = false;  
        }
    }

    public void TakeAlterPower(int Amount)
    {
        if (IsDead) return;
        
        if (Amount > 0)
        {
            RuntimeManager.PlayOneShot(GainPowerSound);
        }
        else if (Amount < 0)
        {
            RuntimeManager.PlayOneShot(LosePowerSound);
        }
        else { return; }

        BonusPower += Amount;
        if (transform != null)
        {
            transform.DOShakePosition(0f, 0.1f);
        }
    }

    public void TakeLifeLoss(int Amount)
    {
        if (IsDead) return;
        if (Amount <= 0) return;

        transform.DOShakePosition(0.2f, 0.5f);
        TriggerEventGA triggerEventGA = new(Events.OnDamaged,null,this,null);
        ActionSystem.Instance.AddReaction(triggerEventGA);
        

        currentLife -= Amount;
        if (currentLife <= 0)
        {
            DiePermanentGA diePermanentGA = new(IsCore, Durability, CardReferenceArchive, this);
            ActionSystem.Instance.AddReaction(diePermanentGA);
            IsDead = true;
        }
        else
        {
            RuntimeManager.PlayOneShot(TakeLifeLossSound);
        }

        UpdateLifeText();
    }

    public void GainLife(int Amount)
    {
        if (IsDead) return;

        if (Amount > 0)
        {
            RuntimeManager.PlayOneShot(BuffLifeSound);
        }
        else if (Amount < 0)
        {
            RuntimeManager.PlayOneShot(DebuffLifeSound);
        }
        else { return; }

        currentLife += Amount;
        MaxLife += Amount;

        if (currentLife <= 0)
        {
            DiePermanentGA diePermanentGA = new(IsCore, Durability, CardReferenceArchive, this);
            ActionSystem.Instance.AddReaction(diePermanentGA);
            IsDead = true;
        }

        UpdateLifeText();
    }

    public void ActiveSelectEffect()
    {
        PermanentSpriteRenderer.color = Color.red;
        RuntimeManager.PlayOneShot(SelectedSound);
    }

    public void RemoveSelectEffect()
    {
        PermanentSpriteRenderer.color = Color.white;
        RuntimeManager.PlayOneShot(UnSelectedSound);
    }

}
