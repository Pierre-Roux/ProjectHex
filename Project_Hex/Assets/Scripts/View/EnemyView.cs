using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyView : MonoBehaviour
{
    [SerializeField] public List<EnemySlotView> Slots;
    [SerializeField] public EnemySlotView CoreSlot;

    public void Setup()
    {
        foreach (var Slot in Slots)
        {
            Slot.setup();
        }
    }

}
