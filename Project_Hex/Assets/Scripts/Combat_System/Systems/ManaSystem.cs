using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaSystem : Singleton<ManaSystem>
{
    [SerializeField] private ManaUI manaUI;
    public int MAX_MANA = 10;
    public int currentMana;

    public void OnEnable()
    {
        ActionSystem.AttachPerformer<SpendManaGA>(SpendManaPerformer);
        ActionSystem.AttachPerformer<ReffilManaGA>(RefillManaPerformer);
    }

    public void OnDisable()
    {
        ActionSystem.DetachPerformer<SpendManaGA>();
        ActionSystem.DetachPerformer<ReffilManaGA>();
    }

    public void SetManaMax(int Amount)
    {
        MAX_MANA = Amount;
    }

    //performers

    private IEnumerator SpendManaPerformer(SpendManaGA spendManaGA)
    {
        currentMana -= spendManaGA.Amount;
        manaUI.UpdateManaText(currentMana);
        yield return null;
    }

    private IEnumerator RefillManaPerformer(ReffilManaGA reffilManaGA)
    {
        currentMana = MAX_MANA;
        manaUI.UpdateManaText(currentMana);
        yield return null;
    }

    public bool HasEnoughMana(int manacost)
    {
        return currentMana >= manacost;
    }
}
