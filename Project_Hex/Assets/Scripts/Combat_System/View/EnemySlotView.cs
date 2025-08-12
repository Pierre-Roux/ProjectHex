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


    [HideInInspector] public Effect IntentAction;
    [HideInInspector] public int currentLife { get; set; }
    [HideInInspector] public bool IsCore { get; set; }
    [HideInInspector] public bool IsDead = false;
    [HideInInspector] public Vector3 InitialPosition { get; set; }

    [HideInInspector] public PermanentType permanentType;

    [HideInInspector] public List<PermanentView> PlayerShielder;
    [HideInInspector] public List<EnemySlotView> EnemyShielder ;
    [HideInInspector] public List<PermanentView> PlayerShielded;
    [HideInInspector] public List<EnemySlotView> EnemyShielded ;
    
    [HideInInspector] public bool Targetable = true;
    [HideInInspector] public bool Shielded;

    [HideInInspector] public bool RDMSequence;
    [HideInInspector] public List<string> IntentSequence = new List<string>();
    [HideInInspector] public bool LoopingSequence;

    private int sequenceIndex = 0;
    private bool CoreLog;

    public void setup()
    {
        PossibleIntent = PermanentData.PossibleIntent;
        spriteRenderer.sprite = PermanentData.PermanentImage;
        currentLife = PermanentData.PermanentLife;
        IsCore = PermanentData.IsCore;
        UnShieldable = PermanentData.UnShieldable;
        ShieldVisual.SetActive(false);
        Targetable = true;
        RDMSequence = PermanentData.RDMSequence;
        IntentSequence = PermanentData.IntentSequence;
        LoopingSequence = PermanentData.LoopingSequence;
        if (IsCore)
        {
            permanentType = PermanentType.none;
            CoreLog = true;
        }
        else
        {
            permanentType = PermanentData.permanentType;
            CoreLog = false;
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
        if (currentLife > PermanentData.PermanentLife)
        {
            currentLife = PermanentData.PermanentLife;
        }
        transform.DOShakePosition(0f, 0.1f);
        UpdateLifeText();
    }

    public void TakeShield(PermanentView playerShielder = null, EnemySlotView enemyShielder = null)
    {
        if (IsCore) Debug.Log("Tentative de shield");
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
                    if (IsCore) Debug.Log("un shield me shield");
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
            if (IsCore) Debug.Log("un shield est tombé");
        }
        UpdateShield();        
    }

    public void UpdateShield()
    {
        if (IsCore) Debug.Log("il reste " + EnemyShielder.Count + " Shielder côté enemy");
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

    public void ActiveSelectEffect()
    {
        spriteRenderer.color = Color.red;
    }

    public void RemoveSelectEffect()
    {
        spriteRenderer.color = Color.white;
    }
}
