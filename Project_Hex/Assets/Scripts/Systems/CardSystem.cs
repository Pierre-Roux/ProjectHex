using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CardSystem : Singleton<CardSystem>
{
    [SerializeField] private HandView handView;
    [SerializeField] private Transform drawPilePoint;
    [SerializeField] private Transform discardPilePoint;

    private readonly List<Card> drawPile = new();
    private readonly List<Card> discardPile = new();
    public readonly List<Card> hand = new();

    void OnEnable()
    {
        ActionSystem.AttachPerformer<DrawCardsGA>(DrawCardsPerformer);
        ActionSystem.AttachPerformer<DiscardAllCardsGA>(DiscardAllCardsPerformer);
        ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);

    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<DrawCardsGA>();
        ActionSystem.DetachPerformer<DiscardAllCardsGA>();
        ActionSystem.DetachPerformer<PlayCardGA>();
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);

    }

    // DECK Setup

    public void Setup(List<CardData> deckdata)
    {
        foreach (var cardData in deckdata)
        {
            Card card = new(cardData);
            drawPile.Add(card);
        }
    }

    // PERFORMERS

    private IEnumerator DrawCardsPerformer(DrawCardsGA drawCardsGA)
    {
        int actualAmount = Mathf.Min(drawCardsGA.Amount, drawPile.Count);
        int notDrawAmount = drawCardsGA.Amount - actualAmount;
        for (int i = 0; i < actualAmount; i++)
        {
            yield return DrawCard();
        }
        if (notDrawAmount > 0)
        {
            RefillDeck();
            if (drawPile.Count < notDrawAmount)
            {
                notDrawAmount = drawPile.Count;
            }
            for (int i = 0; i < notDrawAmount; i++)
            {
                yield return DrawCard();
            }
        }
    }

    private IEnumerator DrawCard()
    {
        Card card = drawPile.Draw();
        hand.Add(card);
        CardView cardView = CardViewCreator.Instance.CreateCardView(card, drawPilePoint.position, drawPilePoint.rotation);
        TriggerEventGA triggerEventGA = new(Events.OnDraw,cardView.Card);
        ActionSystem.Instance.AddReaction(triggerEventGA);
        yield return handView.AddCard(cardView);
    }

    private void RefillDeck()
    {
        drawPile.AddRange(discardPile);
        discardPile.Clear();
    }

    private IEnumerator DiscardAllCardsPerformer(DiscardAllCardsGA discardAllCardsGA)
    {
        foreach (var card in hand)
        {
            CardView cardView = handView.RemoveCard(card);
            yield return DiscardCard(cardView);
        }
        hand.Clear();
    }

    public IEnumerator DiscardCard(CardView cardView)
    {
        TriggerEventGA triggerEventGA = new(Events.OnDiscard,cardView.Card);
        ActionSystem.Instance.AddReaction(triggerEventGA);
        cardView.transform.DOScale(Vector3.zero, 0.15f);
        Tween tween = cardView.transform.DOMove(discardPilePoint.position, 0.15f);
        yield return tween.WaitForCompletion();
        discardPile.Add(cardView.Card);
        Destroy(cardView.gameObject);
    }
    
    public IEnumerator DestroyCard(CardView cardView)
    {
        Tween tween = cardView.transform.DOScale(Vector3.zero, 0.15f);
        //Tween tween = cardView.transform.DOMove(discardPilePoint.position, 0.15f);
        yield return tween.WaitForCompletion();
        Destroy(cardView.gameObject);
    }


    private IEnumerator PlayCardPerformer(PlayCardGA playCardGA)
    {
        // Si on joue une carte toute les event OnPlay ce joue (il faudrait faire des OnPlaySpell, OnPlayPermanent ect...)
        TriggerEventGA triggerEventGA = new(Events.OnPlay);
        ActionSystem.Instance.AddReaction(triggerEventGA);
        hand.Remove(playCardGA.Card);
        CardView cardView = handView.RemoveCard(playCardGA.Card);
        yield return DiscardCard(cardView);


        SpendManaGA spendManaGA = new(playCardGA.Card.cost);
        ActionSystem.Instance.AddReaction(spendManaGA);
        foreach (var effect in playCardGA.Card.Effects)
        {
            if (effect.Events == Events.Instant)
            {
                DoEffectGA performEffectGA = new(effect);
                ActionSystem.Instance.AddReaction(performEffectGA);
            }
            else
            {
                if (effect.Events != Events.OnDeath && effect.Events != Events.OnDestroy && effect.Events != Events.OnDamaged && effect.Events != Events.OnActivate)
                {
                    GameEventSystem.Instance.AddEffectToEvent(effect);
                }
            }
        }
    }

    public IEnumerator InsertCard(CardView card)
    {
        yield return DiscardCard(card);
    }

    // REACTIONS

    private void EnemyTurnPreReaction(EnemyTurnGA enemyTurnGA)
    {
        DiscardAllCardsGA discardAllCardsGA = new();
        ActionSystem.Instance.AddReaction(discardAllCardsGA);
        TriggerEventGA triggerEventGA = new(Events.EndTurn);
        ActionSystem.Instance.AddReaction(triggerEventGA);
    }

    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {
        DrawCardsGA drawCardsGA = new(5);
        ActionSystem.Instance.AddReaction(drawCardsGA);
        TriggerEventGA triggerEventGA = new(Events.StartTurn);
        ActionSystem.Instance.AddReaction(triggerEventGA);
    }
}
