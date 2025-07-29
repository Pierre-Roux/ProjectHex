using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyView : MonoBehaviour
{
    [field: SerializeField] public int Tier;
    [SerializeField] public List<EnemyPermanentData> EnemyPreset;
    [SerializeField] public EnemyZoneView WeaponZone;
    [SerializeField] public EnemyZoneView ShieldZone;
    [SerializeField] public EnemyZoneView SupportZone;
    [SerializeField] public EnemySlotView CoreSlot;

    public void Setup()
    {
        Debug.Log("Starting Setup");
        CoreSlot.setup();
        CombatSystem.Instance.Enemy_Permanents.Add(CoreSlot);

        foreach (EnemyPermanentData enemy in EnemyPreset)
        {
            EnemySlotViewCreator.Instance.CreateEnemySlotViewCreator(enemy, enemy.permanentType);
        }
    }
}
