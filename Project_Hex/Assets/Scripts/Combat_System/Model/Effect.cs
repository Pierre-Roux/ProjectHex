using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public abstract class Effect
{
    [Header("Mandatory")]
    [SerializeField] public ActionnerType actionnerType;
    [SerializeField] public Events Events;

    [Header("Enemy_Only")]
    [SerializeField] public String Intent_Title;
    [SerializeField] public string number;

    [Header("On Delayed Events")]
    [SerializeField] public int Duration;
    [SerializeField] public Events DurationType;

    [HideInInspector] public GameObject Actionner;
    [HideInInspector] public Card CardActionner;

    public abstract GameAction GetGameAction();

    public virtual Effect Clone()
    {
        return null;
    }
}
