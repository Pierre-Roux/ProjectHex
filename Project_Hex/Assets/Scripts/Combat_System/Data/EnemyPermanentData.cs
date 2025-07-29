using System.Collections.Generic;
using SerializeReferenceEditor;
using UnityEngine;

[CreateAssetMenu(menuName = ("Data/EnemyPermanent"))]
public class EnemyPermanentData : ScriptableObject
{
    [field: SerializeField] public Sprite PermanentImage;
    [field: SerializeField] public int PermanentLife;
    [field: SerializeField] public bool IsCore;
    [field: SerializeField] public PermanentType permanentType;
    [field: SerializeReference, SR] public List<Effect> PossibleIntent { get; private set; }
}
