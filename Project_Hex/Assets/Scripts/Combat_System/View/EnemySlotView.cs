using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class EnemySlotView : MonoBehaviour
{

    [HideInInspector] public Effect IntentAction;
    public List<Effect> PossibleIntent;
    public EnemyPermanentData PermanentData;

    [SerializeField] public TMP_Text LifeText;
    [SerializeField] public TMP_Text IntentText;

    [SerializeField] public SpriteRenderer spriteRenderer;

    [HideInInspector] public int currentLife { get; set; }
    [HideInInspector] public bool IsCore { get; set; }
    [HideInInspector] public bool IsDead = false;
    [HideInInspector] public Vector3 InitialPosition { get; set; }

    [HideInInspector] public PermanentType permanentType;

    public void setup()
    {
        PossibleIntent = PermanentData.PossibleIntent;
        spriteRenderer.sprite = PermanentData.PermanentImage;
        currentLife = PermanentData.PermanentLife;
        IsCore = PermanentData.IsCore;
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
        Effect original = null;
        var validIntents = new List<Effect>(PossibleIntent);

        // Évite boucle infinie si aucun élément ne satisfait la condition
        int safety = 100;
        while (validIntents.Count > 0 && safety-- > 0)
        {
            var candidate = validIntents[Random.Range(0, validIntents.Count)];
            if (candidate.Events == Events.Instant)
            {
                original = candidate;
                break;
            }

            validIntents.Remove(candidate);
        }

        if (original == null)
        {
            Debug.LogWarning("Aucun effet avec Events == Instant trouvé.");
        }
        else
        {
            IntentAction = original.Clone();
            IntentAction.Actionner = this.gameObject;

            IntentText.text = original.Intent_Title;
        }
    }

    public void TakeDamage(int Amount)
    {
        if (!IsDead)
        {
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
        transform.DOShakePosition(0.2f, 0.5f);
        UpdateLifeText();
    }

    public void TakeHeal(int Amount)
    {
        currentLife += Amount;
        if (currentLife > PermanentData.PermanentLife)
        {
            currentLife = PermanentData.PermanentLife;
        }
        transform.DOShakePosition(0f, 0.1f);
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
