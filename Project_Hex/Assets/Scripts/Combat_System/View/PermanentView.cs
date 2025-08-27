using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class PermanentView : MonoBehaviour
{
    [SerializeField] SpriteRenderer PermanentSpriteRenderer;
    [SerializeField] TMP_Text HealthText;
    [SerializeField] public GameObject ShieldVisual;
    [SerializeField] public bool UnShieldable;

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
    [HideInInspector] public PermanentType permanentType;

    [HideInInspector] public List<PermanentView> PlayerShielder;
    [HideInInspector] public List<EnemySlotView> EnemyShielder;
    [HideInInspector] public List<PermanentView> PlayerShielded;
    [HideInInspector] public List<EnemySlotView> EnemyShielded;
    [HideInInspector] public bool Targetable = true;
    [HideInInspector] public bool Shielded;
    [HideInInspector] public bool isHollow;
    [HideInInspector] public bool isInvoc;
    [HideInInspector] public bool isDecay;
    [HideInInspector] public bool isArtillery;

    public void Setup(Card cardReference)
    {
        Targetable = true;
        IsCore = false;
        CardReferenceArchive = cardReference;
        PermanentSpriteRenderer.sprite = cardReference.data.PermanentImage;
        baseLife = cardReference.data.life;
        MaxLife = CalculateBonusLife(baseLife);
        currentLife = MaxLife; 
        isInvoc = cardReference.data.isInvoc;
        isArtillery = cardReference.data.isArtillery;
        permanentType = cardReference.data.permanentType;
        UnShieldable = cardReference.UnShieldable;
        DecayCounter = cardReference.DecayCounter;
        if (DecayCounter > 0) isDecay = true;
        ShieldVisual.SetActive(false);
        UpdateLifeText();

        MaxDurability = cardReference.MaxDurability;
        Durability = cardReference.Durability;
        if (MaxDurability > 0)
        {
            if (Durability == 0)
            {
                isHollow = true;
            }
        }

        // affichage graphique du hollow
        if (isHollow)
        {
            Color c = PermanentSpriteRenderer.color;
            c.a = 0.5f;
            PermanentSpriteRenderer.color = c;
        }
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
        permanentType = PermanentType.none;
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
    
    public int CalculateBonusPower(int BaseAmount)
    {
        int PassiveBonus = 0;

        if (isInvoc)
        {
            PassiveBonus += CombatSystem.Instance.Invoc_PlayerGeneralHPGain + CombatSystem.Instance.Invoc_GeneralPower;
        }
        if (isDecay)
        {
            PassiveBonus += CombatSystem.Instance.Decay_PlayerGeneralHPGain + CombatSystem.Instance.Decay_GeneralPower;
        }
        if (isHollow)
        {
            PassiveBonus += CombatSystem.Instance.Hollow_PlayerGeneralHPGain + CombatSystem.Instance.Hollow_GeneralPower;
        }
        if (isArtillery)
        {
            PassiveBonus += CombatSystem.Instance.Artillery_PlayerGeneralHPGain + CombatSystem.Instance.Artillery_GeneralPower;
        }


        int finalDMG = 0;
        finalDMG = BaseAmount + BonusPower + PassiveBonus + CombatSystem.Instance.EnemyGeneralPower + CombatSystem.Instance.GeneralPower; ;
        if (finalDMG < 0) finalDMG = 0;
        return finalDMG;
    }
    public int CalculateBonusLife(int BaseAmount)
    {
        int PassiveBonus = 0;

        if (isInvoc)
        {
            PassiveBonus += CombatSystem.Instance.Invoc_PlayerGeneralHPGain + CombatSystem.Instance.Invoc_GeneralHPGain;
        }
        if (isDecay)
        {
            PassiveBonus += CombatSystem.Instance.Decay_PlayerGeneralHPGain + CombatSystem.Instance.Decay_GeneralHPGain;
        }
        if (isHollow)
        {
            PassiveBonus += CombatSystem.Instance.Hollow_PlayerGeneralHPGain + CombatSystem.Instance.Hollow_GeneralHPGain;
        }
        if (isArtillery)
        {
            PassiveBonus += CombatSystem.Instance.Artillery_PlayerGeneralHPGain + CombatSystem.Instance.Artillery_GeneralHPGain;
        }


        int finalHPGain = 0;
        finalHPGain = BaseAmount + BonusPower + PassiveBonus + CombatSystem.Instance.PlayerGeneralHPGain + CombatSystem.Instance.GeneralHPGain; ;
        if (finalHPGain < 0) finalHPGain = 0;
        return finalHPGain;
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

    public void TakeDamage(int Amount)
    {
        if (Amount <= 0) return;
        currentLife -= Amount;
        UpdateLifeText();

        if (!IsDead)
        {
            transform.DOShakePosition(0.2f, 0.5f);
            TriggerPermanentEventGA triggerPermanentEventGA = new(this, Events.OnDamaged);
            ActionSystem.Instance.AddReaction(triggerPermanentEventGA);
        }

        if (currentLife <= 0)
        {
            if (!IsDead)
            {
                DiePermanentGA diePermanentGA = new(IsCore, Durability, CardReferenceArchive, this);
                ActionSystem.Instance.AddReaction(diePermanentGA);
                IsDead = true;
            }
        }
    }

    public void TakeHeal(int Amount)
    {
        currentLife += Amount;
        if (currentLife > MaxLife)
        {
            currentLife = MaxLife;
        }
        transform.DOShakePosition(0.1f, 0.1f);
        UpdateLifeText();
    }

    public void TakeShield(PermanentView playerShielder = null, EnemySlotView enemyShielder = null)
    {
        if (!UnShieldable)
        {
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
            ShieldVisual.SetActive(false);
            Shielded = false;  
        }
    }

    public void TakeAlterPower(int Amount)
    {
        if (IsDead) return;
        BonusPower += Amount;
        transform.DOShakePosition(0f, 0.1f);
    }

    public void TakeLifeLoss(int Amount)
    {
        if (IsDead) return;
        if (Amount <= 0) return;

        transform.DOShakePosition(0.2f, 0.5f);
        TriggerPermanentEventGA triggerEventGA = new(this, Events.OnDamaged);
        ActionSystem.Instance.AddReaction(triggerEventGA);
        

        currentLife -= Amount;
        if (currentLife <= 0)
        {
            DiePermanentGA diePermanentGA = new(IsCore, Durability, CardReferenceArchive, this);
            ActionSystem.Instance.AddReaction(diePermanentGA);
            IsDead = true;
        }

        UpdateLifeText();
    }

    public void GainLife(int Amount)
    {
        if (IsDead) return;

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
    }

    public void RemoveSelectEffect()
    {
        PermanentSpriteRenderer.color = Color.white;
    }

}
