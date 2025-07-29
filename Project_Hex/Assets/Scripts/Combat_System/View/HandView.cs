using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;

public class HandView : MonoBehaviour
{
    [SerializeField] private SplineContainer SplineContainer;

    private readonly List<CardView> cards = new();

    public CardView RemoveCard(Card card)
    {
        CardView cardView = GetCardView(card);
        if (cardView == null) return null;
        cards.Remove(cardView);
        StartCoroutine(UpdateCardPosition(0.15f));
        return cardView;
    }

    private CardView GetCardView(Card card)
    {
        return cards.Where(CardView => CardView.Card == card).FirstOrDefault();
    }

    public IEnumerator AddCard(CardView cardView)
    {
        cards.Add(cardView);
        yield return UpdateCardPosition(0.15f);
    }

    /*public IEnumerator RemoveCard(int index)
    {
        Debug.Log("index of delete : " + (index-1));
        cards.RemoveAt(index-1);
        Debug.Log("il reste  : " + cards.Count + " carte en main");
        yield return UpdateCardPosition(0.15f);
    }*/

    private IEnumerator UpdateCardPosition(float duration)
    {
        if (cards.Count == 0) yield break;
        float cardSpacing = 1f / 10f;
        float firstCardPosition = 0.5f - (cards.Count - 1) * cardSpacing / 2;
        Spline spline = SplineContainer.Spline;
        for (int i = 0; i < cards.Count; i++)
        {
            float p = firstCardPosition + i * cardSpacing;
            Vector3 SplinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(Vector3.Cross(up, forward).normalized, up);
            cards[i].transform.DOMove(SplinePosition + transform.position + 0.01f * i * Vector3.back, duration);
            cards[i].transform.DORotate(rotation.eulerAngles, duration);
        }

        yield return new WaitForSeconds(duration);
    } 
}
