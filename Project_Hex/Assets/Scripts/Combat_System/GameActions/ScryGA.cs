using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScryGA : GameAction
{
    public int Amount { get; set; }

    public ScryGA(int amount)
    {
        Amount = amount;
    }
}
