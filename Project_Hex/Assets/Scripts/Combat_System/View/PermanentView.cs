using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class PermanentView : MonoBehaviour
{
    [SerializeField] SpriteRenderer PermanentSpriteRenderer;
    [SerializeField] TMP_Text HealthText;

    [HideInInspector] public bool IsCore { get; set; }
    [HideInInspector] private int MaxLife { get; set; }
    [HideInInspector] public int currentLife { get; set; }
    [HideInInspector] public int damage { get; set; }
    [HideInInspector] public int Durability { get; set; }
    [HideInInspector] public Card CardReferenceArchive;
    [HideInInspector] public bool IsDead = false;
    [HideInInspector] public Vector3 InitialPosition { get; set; }
    [HideInInspector] public PermanentType permanentType;

    [HideInInspector] public List<PermanentView> PlayerShielder;
    [HideInInspector] public List<EnemySlotView> EnemyShielder ;
    [HideInInspector] public List<PermanentView> PlayerShielded;
    [HideInInspector] public List<EnemySlotView> EnemyShielded ;
    [SerializeField] public GameObject ShieldVisual ;
    [HideInInspector] public bool Targetable;

    public void Setup(Card cardReference)
    {
        IsCore = false;
        CardReferenceArchive = cardReference;
        PermanentSpriteRenderer.sprite = cardReference.data.PermanentImage;
        MaxLife = cardReference.data.life;
        currentLife = MaxLife;
        permanentType = cardReference.data.permanentType;
        ShieldVisual.SetActive(false);
        Targetable = true;
        UpdateLifeText();

        Durability = cardReference.Durability;

        damage = cardReference.data.damage;
    }

    public void SetPosition(Vector3 pos)
    {
        InitialPosition = pos;
    }

    public void SetupCore(PlayerData CoreData)
    {
        IsCore = true;
        PermanentSpriteRenderer.sprite = CoreData.CoreImage;
        permanentType = PermanentType.none;
        MaxLife = CoreData.CoreHealth;
        currentLife = MaxLife;
        ShieldVisual.SetActive(false);
        Targetable = true;
        UpdateLifeText();
    }

    public void UpdateLifeText()
    {
        HealthText.text = currentLife.ToString();
    }

    public void TakeDamage(int Amount)
    {
        currentLife -= Amount;
        UpdateLifeText();
        transform.DOShakePosition(0.2f, 0.5f);

        if (!IsDead)
        {
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
            Targetable = false;
        }
        else
        {
            ShieldVisual.SetActive(false);
            Targetable = true;  
        }
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
