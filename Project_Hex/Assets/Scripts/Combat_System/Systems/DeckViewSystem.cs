using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class DeckViewSystem : Singleton<DeckViewSystem>
{
    [SerializeField] public GameObject UIDeckViewPanel;
    [SerializeField] public GameObject UIDeckViewPanelContent;
    public void DisplayCards(List<Card> CardsToDisplay)
    {
        CleanDisplay();
        // Instantiate new
        foreach (var card in CardsToDisplay)
        {
            CardView cardView = CardViewCreator.Instance.CreateCardView(card, Vector3.zero, quaternion.identity, UIDeckViewPanelContent.transform);
            cardView.gameObject.GetComponent<SortingGroup>().sortingOrder = 1;
            cardView.transform.DOScale(50, 0.5f);
        }
    }

    public void CleanDisplay()
    {
        // Clean previous
        foreach (Transform child in UIDeckViewPanelContent.transform)
            Destroy(child.gameObject);
    }
}
