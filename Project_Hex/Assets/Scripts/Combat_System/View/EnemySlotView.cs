using System.Collections;
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

    [HideInInspector] public List<PermanentView> PlayerShielder;
    [HideInInspector] public List<EnemySlotView> EnemyShielder ;
    [HideInInspector] public List<PermanentView> PlayerShielded;
    [HideInInspector] public List<EnemySlotView> EnemyShielded ;
    [SerializeField] public GameObject ShieldVisual ;
    [HideInInspector] public bool Targetable;

    [HideInInspector] public bool RDMSequence;
    [HideInInspector] public List<string> IntentSequence = new List<string>();
    [HideInInspector] public bool LoopingSequence;

    private int sequenceIndex = 0;

    public void setup()
    {
        PossibleIntent = PermanentData.PossibleIntent;
        spriteRenderer.sprite = PermanentData.PermanentImage;
        currentLife = PermanentData.PermanentLife;
        IsCore = PermanentData.IsCore;
        ShieldVisual.SetActive(false);
        RDMSequence = PermanentData.RDMSequence;
        IntentSequence = PermanentData.IntentSequence;
        LoopingSequence = PermanentData.LoopingSequence;
        Targetable = true;
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
                Debug.LogWarning($"{name} has no IntentSequence defined.");
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
            IntentText.text = selectedEffect.Intent_Title;
        }
        else
        {
            IntentText.text = "—";
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

    public void TakeShield(PermanentView playerShielder = null, EnemySlotView enemyShielder = null)
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
        spriteRenderer.color = Color.red;
    }

    public void RemoveSelectEffect()
    {
        spriteRenderer.color = Color.white;
    }
}
