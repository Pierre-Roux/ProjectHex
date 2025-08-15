using System.Collections.Generic;
using SerializeReferenceEditor;
using UnityEngine;

[CreateAssetMenu(menuName = ("Data/EnemyPermanent"))]
public class EnemyPermanentData : ScriptableObject
{
    [field: SerializeField] public Sprite PermanentImage;
    [field: SerializeField] public int PermanentLife;
    [field: SerializeField] public bool IsCore;
    [field: SerializeField] public bool UnShieldable;
    [field: SerializeField] public PermanentType permanentType;
    [field: SerializeField] public int DecayCounter;
    [field: SerializeReference, SR] public List<Effect> PossibleIntent { get; private set; }
    [field: SerializeField] public bool RDMSequence;
    [field: SerializeField] public List<string> IntentSequence { get; private set; }
    [field: SerializeField] public bool LoopingSequence;
}
