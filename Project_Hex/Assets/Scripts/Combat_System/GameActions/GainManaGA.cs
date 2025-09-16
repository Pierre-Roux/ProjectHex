using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainManaGA : GameAction
{
    public int GainAmount { get; set; }
    public GainManaGA(int amount)
    {
        GainAmount = amount;
    }
}
