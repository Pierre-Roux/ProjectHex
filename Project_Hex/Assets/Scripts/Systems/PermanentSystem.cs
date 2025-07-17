using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentSystem : Singleton<PermanentSystem>
{
    [SerializeField] private HandView handView;
    [SerializeField] private CardSystem cardSystem;
    public List<CardSlotView> cardSlotViews = new();

    void OnEnable()
    {
        ActionSystem.AttachPerformer<SummonGA>(SummonPermanentPerformer);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<SummonGA>();
    }

    // PERFORMERS (si je veux faire un Perform dans un Performer il faut faire ActionSystem.Instance.AddReaction(GameAction) plutôt que ActionSystem.Instance.Perform(GameAction) )

    private IEnumerator SummonPermanentPerformer(SummonGA summonGA)
    {
        Card cardToSummon = summonGA.cardToInvoke;

        cardSystem.hand.Remove(cardToSummon);
        CardView cardView = handView.RemoveCard(cardToSummon);

        PermanentView permanentView = PermanentViewCreator.Instance.CreatePermanentViewCreator(cardToSummon, summonGA.Slot);
        CombatSystem.Instance.Player_Permanents.Add(permanentView);

        yield return cardSystem.DestroyCard(cardView);

        SpendManaGA spendManaGA = new(summonGA.cardToInvoke.cost);
        ActionSystem.Instance.AddReaction(spendManaGA);

        foreach (var effect in summonGA.cardToInvoke.Effects)
        {
            Effect clonedEffect = effect.Clone();
            clonedEffect.Actionner = permanentView.gameObject;
            
            if (effect.Events == Events.Instant)
            {
                DoEffectGA performEffectGA = new(clonedEffect);
                ActionSystem.Instance.AddReaction(performEffectGA);
            }
            else
            {
                GameEventSystem.Instance.AddEffectToEvent(clonedEffect);
            }
        }

        // Si on joue une carte toute les event OnPlay ce joue (il faudrait faire des OnPlaySpell, OnPlayPermanent ect...)
        TriggerEventGA triggerEventGA = new(Events.OnPlay);
        ActionSystem.Instance.AddReaction(triggerEventGA);
    }

    // REACTIONS
    

    
}
