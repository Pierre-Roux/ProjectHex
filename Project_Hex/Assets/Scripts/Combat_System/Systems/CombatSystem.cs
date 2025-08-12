using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using System.Linq;

public class CombatSystem : Singleton<CombatSystem>
{
    [SerializeField] public PlayerData Player;
    [SerializeField] public PermanentView PlayerCore;
    [SerializeField] private List<GameObject> EnemiesDataBase;

    [HideInInspector] public bool Interactable;

    [SerializeField] public int CurrentTurn;

    [SerializeField] private Transform EnemySpawn;

    [SerializeField] public GameObject EndGameDefeatPanel;
    [SerializeField] public GameObject EndGameVictoryPanel;

    [SerializeField] public ZoneView PlayerWeaponZone;
    [SerializeField] public ZoneView PlayerShieldZone;
    [SerializeField] public ZoneView PlayerSupportZone;
    [HideInInspector] public EnemyZoneView EnemyWeaponZone;
    [HideInInspector] public EnemyZoneView EnemyShieldZone;
    [HideInInspector] public EnemyZoneView EnemySupportZone;

    public EnemyView currentEnemy;

    public List<EnemySlotView> Enemy_Permanents;
    public List<PermanentView> Player_Permanents;

    private bool startFightSubscribed = false;

    public void OnEnable()
    {
        if (!startFightSubscribed)
        {
            ActionSystem.AttachPerformer<DiePermanentGA>(DiePermanentPerformer);
            ActionSystem.AttachPerformer<DieEnemySlotGA>(DieEnemySlotView);
            ActionSystem.AttachPerformer<DestroyPermanentGA>(DestroyPerformer);
            ActionSystem.AttachPerformer<EndCombatGA>(EndCombat);

            ActionSystem.SubscribeReaction<StartFightGA>(StartFightPreReaction, ReactionTiming.PRE);
            ActionSystem.SubscribeReaction<PlayerTurnGA>(PlayerTurnPreReaction, ReactionTiming.PRE);

            ActionSystem.SubscribeReaction<EndEnemyTurnGA>(EndEnemyTurnPostReaction, ReactionTiming.POST);
            ActionSystem.SubscribeReaction<EndPlayerTurnGA>(EndPlayerTurnPostReaction, ReactionTiming.POST);

            startFightSubscribed = true;
        }
    }

    public void OnDisable()
    {
        if (startFightSubscribed)
        {
            ActionSystem.DetachPerformer<DiePermanentGA>();
            ActionSystem.DetachPerformer<DieEnemySlotGA>();
            ActionSystem.DetachPerformer<DestroyPermanentGA>();
            ActionSystem.DetachPerformer<EndCombatGA>();

            ActionSystem.UnsubscribeReaction<StartFightGA>(StartFightPreReaction, ReactionTiming.PRE);
            ActionSystem.UnsubscribeReaction<PlayerTurnGA>(PlayerTurnPreReaction, ReactionTiming.PRE);

            ActionSystem.UnsubscribeReaction<EndEnemyTurnGA>(EndEnemyTurnPostReaction, ReactionTiming.POST);
            ActionSystem.UnsubscribeReaction<EndPlayerTurnGA>(EndPlayerTurnPostReaction, ReactionTiming.POST);

            startFightSubscribed = false;
        }
    }

    private void Start()
    {
        //StartCoroutine(DelayedStartup());
        ClassicStartUp();
    }

    private IEnumerator DelayedStartup()
    {
        yield return null; // attend une frame
        ClassicStartUp();
    }

    // Mise en place classique
    public void ClassicStartUp()
    {
        if (DataBase.Instance.DeckList.Count == 0)
        {
            DataBase.Instance.DeckList = new List<CardData>(Player.deckData);
            DataBase.Instance.INITIALDeckList = new List<CardData>(Player.deckData);
        }
        CardSystem.Instance.Setup(DataBase.Instance.DeckList);
        PlayerCore.SetupCore(Player);

        int stage = 0;
        int targetTier = 0;

        if (DataBase.Instance.CurrentStage <= 0)
        {
            stage = 0;
        }
        else
        {
            stage = DataBase.Instance.CurrentStage;
        }

        if (DataBase.Instance.CoreLife != 0)
        {
            PlayerCore.currentLife = DataBase.Instance.CoreLife;
        }


        // Détermine le Tier selon le Stage
        if (stage < 2)
            targetTier = 0;
        else if (stage == 2)
            targetTier = 1;
        else if (stage < 5)
            targetTier = 2;
        else if (stage == 5)
            targetTier = 3;
        else if (stage < 8)
            targetTier = 4;
        else if (stage == 8)
            targetTier = 5;
        else
            targetTier = 0;

        //if (DataBase.Instance.IsElite)
        //targetTier++;

        // Filtrage
        List<GameObject> validEnemies = EnemiesDataBase
        .Where(e => e.GetComponent<EnemyView>().Tier == targetTier)
        .ToList();

        // Si aucun ennemi trouvé pour ce Tier
        if (validEnemies.Count == 0)
        {
            Debug.LogWarning($"⚠ Aucun ennemi trouvé pour le Tier {targetTier}, sélection aléatoire globale.");
            validEnemies = EnemiesDataBase;
        }

        GameEventSystem.Instance.ClearAllEvents();

        Player_Permanents.Add(PlayerCore);

        // Choix aléatoire
        GameObject selectedEnemy = validEnemies[Random.Range(0, validEnemies.Count - 1)];
        GameObject SpawnedEnemy = Instantiate(selectedEnemy, EnemySpawn.position, EnemySpawn.rotation, EnemySpawn);
        EnemyView enemyView = SpawnedEnemy.GetComponent<EnemyView>();
        currentEnemy = enemyView;
        EnemySystem.Instance.enemyView = enemyView;
        EnemySlotViewCreator.Instance.WeaponZone = EnemyWeaponZone = enemyView.WeaponZone;
        EnemySlotViewCreator.Instance.ShieldZone = EnemyShieldZone = enemyView.ShieldZone;
        EnemySlotViewCreator.Instance.SupportZone = EnemySupportZone = enemyView.SupportZone;
        enemyView.Setup();
        foreach (EnemySlotView enemySlotView in Enemy_Permanents)
        {
            if (enemySlotView.PossibleIntent == null) continue;
            foreach (Effect effect in enemySlotView.PossibleIntent)
            {
                if (effect.Events != Events.EnemyTurn && effect.Events != Events.Instant)
                {
                    Effect clonedEffect = effect.Clone();
                    clonedEffect.Actionner = enemySlotView.gameObject;

                    GameEventSystem.Instance.AddEffectToEvent(clonedEffect);
                }
            }
        }

        StartFightGA startFight = new(enemyView);
        ActionSystem.Instance.Perform(startFight);

        Interactable = true;
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
                    LoseShieldGA loseShieldGA = new(diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(loseShieldGA);

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
                    LoseShieldGA loseShieldGA = new(diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(loseShieldGA);

                    diePermanentGA.CardReferenceArchive.Durability -= 1;
                    CardView newCardView = CardViewCreator.Instance.CreateCardView(diePermanentGA.CardReferenceArchive, diePermanentGA.PermanentView.transform.position, diePermanentGA.PermanentView.transform.rotation);

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
            Interactable = false;
            EndGameDefeatPanel.SetActive(true);
        }
    }

    public IEnumerator DieEnemySlotView(DieEnemySlotGA dieEnemySlotGA)
    {
        LoseShieldGA loseShieldGA = new(null, dieEnemySlotGA.EnemySlotView);
        ActionSystem.Instance.AddReaction(loseShieldGA);

        TriggerEnemyEventGA triggerEnemyEventGA = new(dieEnemySlotGA.EnemySlotView, Events.OnDeath);
        ActionSystem.Instance.AddReaction(triggerEnemyEventGA);

        CombatSystem.Instance.Enemy_Permanents.Remove(dieEnemySlotGA.EnemySlotView);

        DestroyPermanentGA destroyPermanentGA = new(null, dieEnemySlotGA.EnemySlotView);

        if (dieEnemySlotGA.EnemySlotView.IsCore)
        {
            EndCombatGA endCombatGA = new();
            ActionSystem.Instance.AddReaction(endCombatGA);
        }

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

            EnemyWeaponZone.RepositionChildrenEnemySlotView();
            EnemyShieldZone.RepositionChildrenEnemySlotView();
            EnemySupportZone.RepositionChildrenEnemySlotViewCenterOut();
        }

        if (destroyPermanentGA.PermanentView != null)
        {
            GameEventSystem.Instance.RemoveEffectByActionner(destroyPermanentGA.PermanentView.gameObject);
            CombatSystem.Instance.Player_Permanents.Remove(destroyPermanentGA.PermanentView);
            Destroy(destroyPermanentGA.PermanentView.gameObject);

            PlayerWeaponZone.RepositionChildrenPermanentView();
            PlayerShieldZone.RepositionChildrenPermanentView();
            PlayerSupportZone.RepositionChildrenPermanentViewCenterOut();
        }
    }

    public IEnumerator EndCombat(EndCombatGA endCombatGA)
    {
        // Bloque l'interactivité du joeur 
        Interactable = false;
        EndGameVictoryPanel.SetActive(true);
        yield return null;
    }

    // REACTIONS
    private void StartFightPreReaction(StartFightGA startFightGA)
    {
        CurrentTurn = 0;
        foreach (GameAction action in startFightGA.enemyView.SetupActions)
        {
            ActionSystem.Instance.AddReaction(action);
        }
        DeckShuffleGA deckShuffleGA = new();
        ActionSystem.Instance.AddReaction(deckShuffleGA);
        PlayerTurnGA playerTurnGA = new();
        ActionSystem.Instance.AddReaction(playerTurnGA);
    }
    private void PlayerTurnPreReaction(PlayerTurnGA playerTurnGA)
    {
        ReffilManaGA reffilManaGA = new();
        ActionSystem.Instance.AddReaction(reffilManaGA);
        DrawCardsGA drawCardsGA = new(5);
        ActionSystem.Instance.AddReaction(drawCardsGA);
        TriggerEventGA triggerEventGA = new(Events.StartTurn);
        ActionSystem.Instance.AddReaction(triggerEventGA);
    }

    private void EndPlayerTurnPostReaction(EndPlayerTurnGA endPlayerTurnGA)
    {
        TriggerEventGA triggerEventGA = new(Events.EndTurn);
        ActionSystem.Instance.AddReaction(triggerEventGA);
        EnemyTurnGA enemyTurnGA = new();
        ActionSystem.Instance.AddReaction(enemyTurnGA);
    }

    private void EndEnemyTurnPostReaction(EndEnemyTurnGA endEnemyTurnGA)
    {
        PlayerTurnGA playerTurnGA = new();
        ActionSystem.Instance.AddReaction(playerTurnGA);
    }
}
 