using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class PermanentView : MonoBehaviour
{
    [SerializeField] SpriteRenderer PermanentSpriteRenderer;
    [SerializeField] TMP_Text HealthText;
    [SerializeField] TMP_Text ShieldText;

    [HideInInspector] public bool IsCore { get; set; }
    [HideInInspector] private int MaxLife { get; set; }
    [HideInInspector] public int currentLife { get; set; }
    [HideInInspector] public int currentShield { get; set; }
    [HideInInspector] public int damage { get; set; }
    [HideInInspector] public int Durability { get; set; }
    [HideInInspector] public Card CardReferenceArchive;
    [HideInInspector] public CardSlotView CurrentSlot;
    [HideInInspector] public bool IsDead = false;
    [HideInInspector] public Transform InitialPosition { get; set; }

    public void Setup(Card cardReference)
    {
        IsCore = false;
        CardReferenceArchive = cardReference;
        PermanentSpriteRenderer.sprite = cardReference.data.PermanentImage;
        MaxLife = cardReference.data.life;
        currentLife = MaxLife;
        UpdateLifeText();
        currentShield = cardReference.data.StartingShield;
        UpdateShieldText();
        Durability = cardReference.Durability;
        InitialPosition = CurrentSlot.transform;

        damage = cardReference.data.damage;
    }

    public void SetupCore(PlayerData CoreData)
    {
        IsCore = true;
        PermanentSpriteRenderer.sprite = CoreData.CoreImage;
        MaxLife = CoreData.CoreHealth;
        currentLife = MaxLife;
        UpdateLifeText();
        currentShield = CoreData.StartingShield;
        UpdateShieldText();
    }

    public void UpdateLifeText()
    {
        HealthText.text = currentLife.ToString();
    }

    public void UpdateShieldText()
    {
        ShieldText.text = currentShield.ToString();
    }

    public void TakeDamage(int Amount)
    {
        if (currentShield - Amount >= 0)
        {
            currentShield -= Amount;
            UpdateShieldText();
            transform.DOShakePosition(0.05f, 0.05f);
            if (!IsDead)
            {
                TriggerPermanentEventGA triggerPermanentEventGA = new(this, Events.OnDamaged);
                ActionSystem.Instance.AddReaction(triggerPermanentEventGA);
            }
        }
        else
        {
            Amount -= currentShield;
            currentShield = 0;
            UpdateShieldText();

            currentLife -= Amount;
            UpdateLifeText();
            transform.DOShakePosition(0.2f, 0.5f);

            if (!IsDead)
            {
                TriggerPermanentEventGA triggerPermanentEventGA = new(this, Events.OnDamaged);
                ActionSystem.Instance.AddReaction(triggerPermanentEventGA);
            }

            if (IsCore) return;
            if (currentLife <= 0)
            {
                if (!IsDead)
                {
                    DiePermanentGA diePermanentGA = new(IsCore, Durability, CardReferenceArchive, CurrentSlot, this);
                    ActionSystem.Instance.AddReaction(diePermanentGA);
                    IsDead = true;
                }
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

    public void TakeShield(int Amount)
    {
        currentShield += Amount;
        transform.DOShakePosition(0.1f, 0.1f);
        UpdateShieldText();
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
