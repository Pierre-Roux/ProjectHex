using System.Collections.Generic;
using SerializeReferenceEditor;
using UnityEngine;

[CreateAssetMenu(menuName = ("Data/EnemyPermanent"))]
public class EnemyPermanentData : ScriptableObject
{
    [field: SerializeField] public Sprite PermanentImage;
    [field: SerializeField] public int PermanentLife;
    [field: SerializeField] public int StartingShield;
    [field: SerializeField] public bool IsCore;
    [field: SerializeReference, SR] public List<Effect> PossibleIntent { get; private set; }
}
