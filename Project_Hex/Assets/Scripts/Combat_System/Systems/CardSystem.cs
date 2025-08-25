using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class CardSystem : Singleton<CardSystem>
{
    [SerializeField] private HandView handView;
    [SerializeField] private Transform drawPilePoint;
    [SerializeField] private Transform discardPilePoint;

    [SerializeField] private DeckView DrawDeck;
    [SerializeField] private DeckView DiscardDeck;

    public List<Card> drawPile = new();
    public List<Card> discardPile = new();
    public List<Card> hand = new();

    void OnEnable()
    {
        ActionSystem.AttachPerformer<DrawCardsGA>(DrawCardsPerformer);
        ActionSystem.AttachPerformer<DiscardAllCardsGA>(DiscardAllCardsPerformer);
        ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);
        ActionSystem.AttachPerformer<DeckShuffleGA>(DeckShuffleGA);
        ActionSystem.SubscribeReaction<EndPlayerTurnGA>(EndPlayerTurnPreReaction, ReactionTiming.PRE);
        

    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<DrawCardsGA>();
        ActionSystem.DetachPerformer<DiscardAllCardsGA>();
        ActionSystem.DetachPerformer<PlayCardGA>();
        ActionSystem.DetachPerformer<DeckShuffleGA>();
        ActionSystem.UnsubscribeReaction<EndPlayerTurnGA>(EndPlayerTurnPreReaction, ReactionTiming.PRE);

    }

    // DECK Setup

    public void Setup(List<CardData> deckdata)
    {
        foreach (var cardData in deckdata)
        {
            Card card = new(cardData);
            drawPile.Add(card);
        }
        UpdatePiles();
    }

    // PERFORMERS

    private IEnumerator DeckShuffleGA(DeckShuffleGA deckShuffleGA)
    {
        drawPile.Shuffle();
        yield return null;
    }

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
            drawPile.Shuffle();
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
        UpdatePiles();
        hand.Add(card);
        CardView cardView = CardViewCreator.Instance.CreateCardView(card, drawPilePoint.position, drawPilePoint.rotation);
        TriggerEventGA triggerEventGA = new(Events.OnDraw, cardView.Card);
        ActionSystem.Instance.AddReaction(triggerEventGA);
        yield return handView.AddCard(cardView);
    }

    private void RefillDeck()
    {
        drawPile.AddRange(discardPile);
        discardPile.Clear();
        UpdatePiles();
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
        TriggerEventGA triggerEventGA = new(Events.OnDiscard, cardView.Card);
        ActionSystem.Instance.AddReaction(triggerEventGA);
        cardView.transform.DOScale(Vector3.zero, 0.15f);
        Tween tween = cardView.transform.DOMove(discardPilePoint.position, 0.15f);
        yield return tween.WaitForCompletion();
        discardPile.Add(cardView.Card);
        UpdatePiles();
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
        TriggerEventGA triggerEventGA = new(Events.OnPlayCard);
        ActionSystem.Instance.AddReaction(triggerEventGA);
        hand.Remove(playCardGA.Card);
        CardView cardView = handView.RemoveCard(playCardGA.Card);
        yield return DiscardCard(cardView);


        SpendManaGA spendManaGA = new(playCardGA.Card.cost);
        ActionSystem.Instance.AddReaction(spendManaGA);
        foreach (var effect in playCardGA.Card.Effects)
        {
            // On clone l’effet de base pour éviter les références partagées
            Effect clonedEffect = effect.Clone();
            clonedEffect.Actionner = null;

            while (clonedEffect != null)
            {
                if (clonedEffect.Events == Events.Instant)
                {
                    // Exécution immédiate via GameAction
                    ActionSystem.Instance.AddReaction(clonedEffect.GetGameAction());
                }
                else
                {
                    // Ajout aux Events (sauf cas spéciaux)
                    if (clonedEffect.Events != Events.OnDeath &&
                        clonedEffect.Events != Events.OnDestroy &&
                        clonedEffect.Events != Events.OnDamaged &&
                        clonedEffect.Events != Events.OnActivate &&
                        clonedEffect.Events != Events.EnemyTurn &&
                        clonedEffect.Events != Events.Instant)
                    {
                        GameEventSystem.Instance.AddEffectToEvent(clonedEffect);
                    }
                }

                // On lie le parent au linked effect (utile si la chaîne est clonée)
                if (clonedEffect.LinkedEffect != null)
                {
                    clonedEffect.LinkedEffect.ParentEffect = clonedEffect;
                }

                // Avancer dans la chaîne
                clonedEffect = clonedEffect.LinkedEffect;
            }
        }
    }

    public IEnumerator InsertCard(CardView card)
    {
        yield return DiscardCard(card);
    }

    // REACTIONS

    private void EndPlayerTurnPreReaction(EndPlayerTurnGA endPlayerTurnGA)
    {
        DiscardAllCardsGA discardAllCardsGA = new();
        ActionSystem.Instance.AddReaction(discardAllCardsGA);
    }

    public void UpdatePiles()
    {
        DrawDeck.UpdateDeckData(drawPile);
        DiscardDeck.UpdateDeckData(discardPile);
    }
}
