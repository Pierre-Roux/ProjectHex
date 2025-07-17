using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DrawCardsGA : GameAction
{
    public int Amount { get; set; }

    public DrawCardsGA(int amount)
    {
        Amount = amount;
    }
}
