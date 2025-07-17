using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Player")]
public class PlayerData : ScriptableObject
{
    [field: Header("Mandatory")]
    [field: SerializeField] public String Name;
    [field: SerializeField] public int CoreHealth;
    [field: SerializeField] public int StartingShield;
    [field: SerializeField] public Sprite CoreImage;
    [field: SerializeField] public List<CardData> deckData;
}
