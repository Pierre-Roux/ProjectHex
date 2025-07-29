using DG.Tweening;
using UnityEngine;

public class CardViewCreator : Singleton<CardViewCreator>
{
    [SerializeField] private CardView cardViewPrefab;

    public CardView CreateCardView(Card Card, Vector3 position, Quaternion rotation, Transform Parent = null)
    {
        CardView cardView = Instantiate(cardViewPrefab, position, rotation, Parent);
        cardView.transform.localScale = Vector3.zero;
        cardView.transform.DOScale(cardViewPrefab.transform.localScale, 0.15f);
        cardView.Setup(Card);
        foreach (Effect effect in Card.Effects)
        {
            effect.CardActionner = Card;
        }
        return cardView;
    }
}
