using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaSystem : Singleton<ManaSystem>
{
    [SerializeField] private ManaUI manaUI;
    private const int MAX_MANA = 5;
    private int currentMana = MAX_MANA;

    public void OnEnable()
    {
        ActionSystem.AttachPerformer<SpendManaGA>(SpendManaPerformer);
        ActionSystem.AttachPerformer<ReffilManaGA>(RefillManaPerformer);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    public void OnDisable()
    {
        ActionSystem.DetachPerformer<SpendManaGA>();
        ActionSystem.DetachPerformer<ReffilManaGA>();
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
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

    // Reactions

    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {
        ReffilManaGA reffilManaGA = new();
        ActionSystem.Instance.AddReaction(reffilManaGA);
    }
}
