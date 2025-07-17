using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public abstract class Effect
{
    [SerializeField] public ActionnerType actionnerType;
    [SerializeField] public Events Events;
    [SerializeField] public String Intent_Title;

    [HideInInspector] public GameObject Actionner;
    [HideInInspector] public Card CardActionner;

    public abstract GameAction GetGameAction();

    public virtual Effect Clone()
    {
        return null;
    }
}
