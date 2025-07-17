using UnityEngine;

public class CardSlotView : MonoBehaviour
{
    public PermanentView currentPermanent;

    private void OnTriggerEnter(Collider other)
    {
        CardView card = other.GetComponent<CardView>();
        if (card != null && card.isDragging)
        {
            card.Active_cardSlot = this;
            //Debug.Log(" In " + this.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CardView card = other.GetComponent<CardView>();
        if (card != null)
        {
            card.Active_cardSlot = null;
            //Debug.Log(" Out " + this.gameObject.name);
        }
    }
}
