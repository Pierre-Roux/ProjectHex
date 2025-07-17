using UnityEngine;

public class CardViewHover : Singleton<CardViewHover>
{
    [SerializeField] private CardView cardViewToHover;

    public void Show(Card card, Vector3 position)
    {
        cardViewToHover.gameObject.SetActive(true);
        cardViewToHover.Setup(card);
        cardViewToHover.transform.position = position;
    }

    public void Hide()
    {
        cardViewToHover.gameObject.SetActive(false);
    }
}
