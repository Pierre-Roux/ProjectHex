using System.Collections.Generic;
using SerializeReferenceEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Card")]

public class CardData : ScriptableObject
{
    [field: Header("Mandatory")]
    [field: SerializeField] public string Title { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public int cost { get; private set; }
    [field: SerializeField] public Sprite Image { get; private set; }

    [field: Header("Permanent")]
    [field: SerializeField] public int life { get; private set; }
    [field: SerializeField] public int StartingShield { get; private set; }
    [field: SerializeField] public int damage { get; private set; }
    [field: SerializeField] public int Durability { get; private set; }
    [field: SerializeField] public Sprite PermanentImage { get; private set; }
    //[field: SerializeField] public 

    [field: Header("Spell")]
    [field: SerializeField] public bool IsSpell { get; private set; }
    [field: SerializeReference, SR] public List<Effect> Effects { get; private set; }
}
