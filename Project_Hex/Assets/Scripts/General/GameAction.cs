using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public abstract class GameAction
{
    [HideInInspector] public GameObject Actionner;

    public List<GameAction> PreReactions { get; private set; } = new();
    public List<GameAction> PerformReactions { get; private set; } = new();
    public List<GameAction> PostReactions { get; private set; } = new();
}
