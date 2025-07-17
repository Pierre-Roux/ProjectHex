using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiePermanentGA : GameAction
{

    public bool IsCore { get; set; }
    public int Durability { get; set; }
    public Card CardReferenceArchive { get; set; }
    public CardSlotView CurrentSlot { get; set; }
    public PermanentView PermanentView { get; set; }

    public DiePermanentGA(bool isCore, int durability, Card cardReferenceArchive, CardSlotView currentSlot, PermanentView permanentView)
    {
        IsCore = isCore;
        Durability = durability;
        CardReferenceArchive = cardReferenceArchive;
        CurrentSlot = currentSlot;
        PermanentView = permanentView;
    }

}
