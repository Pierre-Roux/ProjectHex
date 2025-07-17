using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CombatSystem : Singleton<CombatSystem>
{
    [SerializeField] private PlayerData Player;
    [SerializeField] private PermanentView PlayerCore;
    [SerializeField] private List<GameObject> enemies;
    [SerializeField] private Transform EnemySpawn;

    private EnemyView currentEnemy;

    public List<EnemySlotView> Enemy_Permanents;
    public List<PermanentView> Player_Permanents;

    public void OnEnable()
    {
        ActionSystem.AttachPerformer<DiePermanentGA>(DiePermanentPerformer);
        ActionSystem.AttachPerformer<DieEnemySlotGA>(DieEnemySlotView);
        ActionSystem.AttachPerformer<DestroyPermanentGA>(DestroyPerformer);
        ActionSystem.SubscribeReaction<StartFightGA>(StartFightPreReaction, ReactionTiming.PRE);
    }

    public void OnDisable()
    {
        ActionSystem.DetachPerformer<DiePermanentGA>();
        ActionSystem.DetachPerformer<DieEnemySlotGA>();
        ActionSystem.DetachPerformer<DestroyPermanentGA>();
        ActionSystem.UnsubscribeReaction<StartFightGA>(StartFightPreReaction, ReactionTiming.PRE);
    }

    private void Start()
    {
        CardSystem.Instance.Setup(Player.deckData);
        PlayerCore.SetupCore(Player);

        GameObject selectedEnemy = enemies[Random.Range(0, enemies.Count - 1)];
        GameObject SpawnedEnemy = Instantiate(selectedEnemy, EnemySpawn.position, EnemySpawn.rotation);
        EnemyView enemyView = SpawnedEnemy.GetComponent<EnemyView>();
        currentEnemy = enemyView;
        EnemySystem.Instance.enemyView = enemyView;
        enemyView.Setup();
        foreach (EnemySlotView Slot in enemyView.Slots)
        {
            if (Slot.PossibleIntent == null) continue;
            foreach (Effect effect in Slot.PossibleIntent)
            {
                if (effect.Events != Events.Instant)
                {
                    Effect clonedEffect = effect.Clone();
                    clonedEffect.Actionner = Slot.gameObject;

                    GameEventSystem.Instance.AddEffectToEvent(clonedEffect);
                }
            }
        }

        StartFightGA startFight = new();
        ActionSystem.Instance.Perform(startFight);

        foreach (EnemySlotView Slot in enemyView.Slots)
        {
            Enemy_Permanents.Add(Slot);
        }
        Player_Permanents.Add(PlayerCore);
    }

    // PERFORMER

    public IEnumerator DiePermanentPerformer(DiePermanentGA diePermanentGA)
    {
        if (!diePermanentGA.IsCore)
        {
            if (diePermanentGA.Durability <= 0)
            {
                if (diePermanentGA.PermanentView != null)
                {
                    TriggerPermanentEventGA triggerPermanentEventGA = new(diePermanentGA.PermanentView, Events.OnDestroy);
                    ActionSystem.Instance.AddReaction(triggerPermanentEventGA);

                    CombatSystem.Instance.Player_Permanents.Remove(diePermanentGA.PermanentView);

                    DestroyPermanentGA destroyPermanentGA = new(diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(destroyPermanentGA);
                }
            }
            else
            {
                if (diePermanentGA.PermanentView != null)
                {
                    diePermanentGA.CardReferenceArchive.Durability -= 1;
                    CardView newCardView = CardViewCreator.Instance.CreateCardView(diePermanentGA.CardReferenceArchive, diePermanentGA.PermanentView.transform.position, diePermanentGA.PermanentView.transform.rotation);
                    diePermanentGA.CurrentSlot.currentPermanent = null;

                    TriggerPermanentEventGA triggerPermanentEventGA = new(diePermanentGA.PermanentView, Events.OnDeath);
                    ActionSystem.Instance.AddReaction(triggerPermanentEventGA);

                    DestroyPermanentGA destroyPermanentGA = new(diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(destroyPermanentGA);

                    newCardView.transform.DOScale(0, 0.01f);
                    Tween tween = newCardView.transform.DOScale(0.4f, 0.2f);
                    yield return tween.WaitForCompletion();
                    yield return new WaitForSeconds(1);
                    yield return CardSystem.Instance.InsertCard(newCardView);
                }
            }
        }
        else
        {
            Debug.Log("CoreDied");
        }
    }

    public IEnumerator DieEnemySlotView(DieEnemySlotGA dieEnemySlotGA)
    {
        TriggerEnemyEventGA triggerEnemyEventGA = new(dieEnemySlotGA.EnemySlotView, Events.OnDeath);
        ActionSystem.Instance.AddReaction(triggerEnemyEventGA);

        CombatSystem.Instance.Enemy_Permanents.Remove(dieEnemySlotGA.EnemySlotView);
        currentEnemy.Slots.Remove(dieEnemySlotGA.EnemySlotView);

        DestroyPermanentGA destroyPermanentGA = new(null,dieEnemySlotGA.EnemySlotView);
        ActionSystem.Instance.AddReaction(destroyPermanentGA);

        yield return null;       
    }

    public IEnumerator DestroyPerformer(DestroyPermanentGA destroyPermanentGA)
    {
        yield return null;
        if (destroyPermanentGA.enemySlotView != null)
        {
            GameEventSystem.Instance.RemoveEffectByActionner(destroyPermanentGA.enemySlotView.gameObject);
            CombatSystem.Instance.Enemy_Permanents.Remove(destroyPermanentGA.enemySlotView);
            Destroy(destroyPermanentGA.enemySlotView.gameObject);
        }

        if (destroyPermanentGA.PermanentView != null)
        {
            GameEventSystem.Instance.RemoveEffectByActionner(destroyPermanentGA.PermanentView.gameObject);
            CombatSystem.Instance.Player_Permanents.Remove(destroyPermanentGA.PermanentView);
            Destroy(destroyPermanentGA.PermanentView.gameObject);
        }
    }

    // REACTIONS

    private void StartFightPreReaction(StartFightGA startFightGA)
    {
        ReffilManaGA reffilManaGA = new();
        ActionSystem.Instance.AddReaction(reffilManaGA);
        DrawCardsGA drawCardsGA = new(5);
        ActionSystem.Instance.AddReaction(drawCardsGA);
    }

}
 