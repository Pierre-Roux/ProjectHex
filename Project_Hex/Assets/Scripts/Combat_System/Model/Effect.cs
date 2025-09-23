using SerializeReferenceEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using FMODUnity;

[System.Serializable]
public abstract class Effect
{
    [Header("Mandatory")]
    [SerializeField] public ActionnerType actionnerType;
    [SerializeField] public Events Events;
    [SerializeField] public bool HollowEffect;
    [SerializeField] public EventReference SFX;

    [Header("Enemy_Only")]
    [SerializeField] public String Intent_Title;
    [SerializeField] public string number;

    [Header("On Delayed Events")]

    [SerializeField] public int Duration;
    [SerializeField] public Events DurationType;
    [SerializeField] public bool TriggerOnDurationEnd;
    [SerializeField] public bool CancelOnDeath;

    [Header("On Condition Effect")]
    public DynamicCondition DynamicCondition;
    public int TestValue;
    public DynamicAmount TestDynamicAmount;
    public PermaTypes TestType;

    [field: SerializeReference, SR] public Effect LinkedEffect;

    [HideInInspector] public GameObject Actionner;
    [HideInInspector] public CardView CardActionner;
    [HideInInspector] public List<PermanentView> TargetForLinked_Player;
    [HideInInspector] public List<EnemySlotView> TargetForLinked_Enemy;
    [HideInInspector] public Effect ParentEffect;

    public abstract GameAction GetGameAction();

    public virtual Effect Clone()
    {
        return null;
    }
}
