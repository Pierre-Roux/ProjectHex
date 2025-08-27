using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class EnemySlotView : MonoBehaviour
{
    [SerializeField] public List<Effect> PossibleIntent;
    [SerializeField] public EnemyPermanentData PermanentData;

    [SerializeField] public TMP_Text LifeText;
    [SerializeField] public TMP_Text IntentText;

    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] public GameObject ShieldVisual ;
    [SerializeField] public bool UnShieldable;
    [SerializeField] public bool isInvoc;


    [HideInInspector] public Effect IntentAction;
    [HideInInspector] public int currentLife { get; set; }
    [HideInInspector] public int baseLife { get; set; }
    [HideInInspector] public int MaxLife { get; set; }
    [HideInInspector] public bool IsCore { get; set; }
    [HideInInspector] public bool IsDead = false;
    [HideInInspector] public bool isDecay = false;
    [HideInInspector] public Vector3 InitialPosition { get; set; }
    [HideInInspector] public int DecayCounter { get; set; }
    [HideInInspector] public int BonusPower { get; set; }
    [HideInInspector] public int CurrentHPBonus { get; set; }

    [HideInInspector] public PermanentType permanentType;

    [HideInInspector] public List<PermanentView> PlayerShielder;
    [HideInInspector] public List<EnemySlotView> EnemyShielder ;
    [HideInInspector] public List<PermanentView> PlayerShielded;
    [HideInInspector] public List<EnemySlotView> EnemyShielded ;
    
    [HideInInspector] public bool Targetable = true;
    [HideInInspector] public bool Shielded;
    [HideInInspector] public bool Activated;

    [HideInInspector] public bool RDMSequence;
    [HideInInspector] public List<string> IntentSequence = new List<string>();
    [HideInInspector] public bool LoopingSequence;

    private int sequenceIndex = 0;

    public void setup()
    {
        PossibleIntent = PermanentData.PossibleIntent;
        spriteRenderer.sprite = PermanentData.PermanentImage;
        baseLife = PermanentData.PermanentLife;
        MaxLife = CalculateBonusLife(baseLife);
        currentLife = MaxLife; 
        IsCore = PermanentData.IsCore;
        UnShieldable = PermanentData.UnShieldable;
        isInvoc = PermanentData.IsInvoc;
        ShieldVisual.SetActive(false);
        Targetable = true;
        RDMSequence = PermanentData.RDMSequence;
        IntentSequence = PermanentData.IntentSequence;
        LoopingSequence = PermanentData.LoopingSequence;
        DecayCounter = PermanentData.DecayCounter;
        if (DecayCounter > 0) isDecay = true;
        if (IsCore)
        {
            permanentType = PermanentType.none;
        }
        else
        {
            permanentType = PermanentData.permanentType;
        }
        UpdateIntent();
        UpdateLifeText();
    }

    public void SetPosition(Vector3 pos)
    {
        InitialPosition = pos;
    }

    public void UpdateLifeText()
    {
        LifeText.text = currentLife.ToString();
    }

    public void UpdateIntent()
    {
        if (PossibleIntent.Count <= 0) return;
        Effect selectedEffect = null;

        if (RDMSequence)
        {
            List<Effect> valid = PossibleIntent.FindAll(e => e.Events == Events.EnemyTurn);

            if (valid.Count > 0)
            {
                selectedEffect = valid[UnityEngine.Random.Range(0, valid.Count)];
            }
        }
        else
        {
            if (IntentSequence.Count == 0)
            {
                return;
            }

            if (sequenceIndex >= IntentSequence.Count)
            {
                if (LoopingSequence)
                    sequenceIndex = 0;
                else
                    return;
            }

            string currentKey = IntentSequence[sequenceIndex];
            if (currentKey != "")
            {
                selectedEffect = PossibleIntent.Find(e => e.Events == Events.EnemyTurn && e.number == currentKey);

                if (selectedEffect == null)
                {
                    Debug.LogWarning($"No matching Effect with number '{currentKey}' in {name}");
                }
            }
            sequenceIndex++;
        }

        if (selectedEffect != null)
        {
            IntentAction = selectedEffect.Clone();
            IntentAction.Actionner = this.gameObject;
            UpdateIntentText(selectedEffect);
        }
        else
        {
            IntentText.text = "!";
        }
    }

    public void UpdateIntentText(Effect selectedEffect)
    {
        if (selectedEffect == null) return;

        string intentText = selectedEffect.Intent_Title; // fallback

        switch (selectedEffect)
        {
            case DealDamageEffect dmg:
                int damagetext = CalculateBonusPower(dmg.damageAmount);

                intentText = $"Deal {damagetext} damage to {dmg.targetMode}";
                break;

            case HealEffect heal:
                intentText = $"Heal {heal.amount} HP to {heal.targetMode}";
                break;

            case DrawCardsEffect draw:
                intentText = $"Draw {draw.drawAmount} cards";
                break;

            case ShieldEffect shield:
                intentText = $"Shield {shield.targetMode} ";
                break;

            case AlterPowerEffect alter:
                intentText = $"Alter power by {alter.alterAmount} of {alter.targetMode}";
                break;
        }

        IntentText.text = intentText;
    }

    public int CalculateBonusPower(int BaseAmount)
    {
        int PassiveBonus = 0;

        if (isInvoc)
        {
            PassiveBonus += CombatSystem.Instance.Invoc_EnemyGeneralPower + CombatSystem.Instance.Invoc_GeneralPower;
        }
        if (isDecay)
        {
            PassiveBonus += CombatSystem.Instance.Decay_EnemyGeneralPower + CombatSystem.Instance.Decay_GeneralPower;
        }
        /*if (isHollow)
        {
            PassiveBonus += CombatSystem.Instance.Hollow_EnemyGeneralPower + CombatSystem.Instance.Hollow_GeneralPower;
        }
        if (isArtillery)
        {
            PassiveBonus += CombatSystem.Instance.Artillery_EnemyGeneralPower + CombatSystem.Instance.Artillery_GeneralPower;
        }*/


        int finalDMG = 0;
        finalDMG = BaseAmount + BonusPower + PassiveBonus + CombatSystem.Instance.EnemyGeneralPower + CombatSystem.Instance.GeneralPower;;
        if (finalDMG < 0) finalDMG = 0;
        return finalDMG;
    }

    public int CalculateBonusLife(int BaseAmount)
    {
        int PassiveBonus = 0;

        if (isInvoc)
        {
            PassiveBonus += CombatSystem.Instance.Invoc_EnemyGeneralHPGain + CombatSystem.Instance.Invoc_GeneralHPGain;
        }
        if (isDecay)
        {
            PassiveBonus += CombatSystem.Instance.Decay_EnemyGeneralHPGain + CombatSystem.Instance.Decay_GeneralHPGain;
        }
        /*if (isHollow)
        {
            PassiveBonus += CombatSystem.Instance.Hollow_EnemyGeneralHPGain + CombatSystem.Instance.Hollow_GeneralHPGain;
        }
        if (isArtillery)
        {
            PassiveBonus += CombatSystem.Instance.Artillery_EnemyGeneralHPGain + CombatSystem.Instance.Artillery_GeneralHPGain;
        }*/


        int finalHPGain = 0;
        finalHPGain = BaseAmount + BonusPower + PassiveBonus + CombatSystem.Instance.EnemyGeneralHPGain + CombatSystem.Instance.GeneralHPGain; ;
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
        if (!IsDead)
        {
            transform.DOShakePosition(0.2f, 0.5f);
            TriggerEnemyEventGA triggerEventGA = new(this, Events.OnDamaged);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }

        currentLife -= Amount;
        if (currentLife <= 0)
        {
            if (!IsDead)
            {
                DieEnemySlotGA dieEnemySlotGA = new(this);
                ActionSystem.Instance.AddReaction(dieEnemySlotGA);
                IsDead = true;
            }
        }

        UpdateLifeText();
    }

    public void TakeHeal(int Amount)
    {
        currentLife += Amount;
        if (currentLife > MaxLife)
        {
            currentLife = MaxLife;
        }
        transform.DOShakePosition(0f, 0.1f);
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
                    playerShielder.GetComponent<PermanentView>().EnemyShielded.Add(this);
                }
            }

            if (enemyShielder != null)
            {
                if (!EnemyShielder.Contains(enemyShielder))
                {
                    EnemyShielder.Add(enemyShielder);
                    enemyShielder.GetComponent<EnemySlotView>().EnemyShielded.Add(this);
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
        if (transform != null)
        {
            transform.DOShakePosition(0f, 0.1f);
        }
        UpdateIntentText(IntentAction);
    }

    public void TakeLifeLoss(int Amount)
    {
        if (IsDead) return;
        if (Amount <= 0) return;

        transform.DOShakePosition(0.2f, 0.5f);
        TriggerEnemyEventGA triggerEventGA = new(this, Events.OnDamaged);
        ActionSystem.Instance.AddReaction(triggerEventGA);
        

        currentLife -= Amount;
        if (currentLife <= 0)
        {
            DieEnemySlotGA dieEnemySlotGA = new(this);
            ActionSystem.Instance.AddReaction(dieEnemySlotGA);
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
            DieEnemySlotGA dieEnemySlotGA = new(this);
            ActionSystem.Instance.AddReaction(dieEnemySlotGA);
            IsDead = true;
        }

        UpdateLifeText();
    }

    public void ActiveSelectEffect()
    {
        spriteRenderer.color = Color.red;
    }

    public void RemoveSelectEffect()
    {
        spriteRenderer.color = Color.white;
    }
}
