using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonGA : GameAction
{
    public Card cardToInvoke;
    public CardSlotView Slot;

    public SummonGA(Card card, CardSlotView slot)
    {
        cardToInvoke = card;
        Slot = slot;
    }
}