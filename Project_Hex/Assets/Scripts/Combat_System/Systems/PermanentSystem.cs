using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class PermanentSystem : Singleton<PermanentSystem>
{
    [SerializeField] private HandView handView;
    [SerializeField] private CardSystem cardSystem;

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

        if (!AudioManager.Instance.IsValid(cardToSummon.PlayCardSound))
        {
            RuntimeManager.PlayOneShot(AudioManager.Instance.PlayCardSound);
        }
        else
        {
            RuntimeManager.PlayOneShot(cardToSummon.PlayCardSound);
        }

        PermanentView permanentView = PermanentViewCreator.Instance.CreatePermanentViewCreator(cardToSummon, cardToSummon.permanentArea);
        CombatSystem.Instance.Player_Permanents.Add(permanentView);

        yield return cardSystem.DestroyCard(cardView);

        SpendManaGA spendManaGA = new(summonGA.cardToInvoke.cost);
        ActionSystem.Instance.AddReaction(spendManaGA);

        foreach (var effect in summonGA.cardToInvoke.Effects)
        {
            // Vérifie Hollow
            bool canApply = (permanentView.permaTypes.Contains(PermaTypes.Hollow) && effect.HollowEffect)
                        || (!permanentView.permaTypes.Contains(PermaTypes.Hollow) && !effect.HollowEffect);
            if (!canApply) continue;

            // On démarre par l’effet cloné
            Effect clonedEffect = effect.Clone();

            while (clonedEffect != null)
            {
                if (clonedEffect.Events == Events.Instant)
                {
                    clonedEffect.Actionner = permanentView.gameObject;
                    DoEffectGA performEffectGA = new(clonedEffect);
                    ActionSystem.Instance.AddReaction(performEffectGA);
                }
                else
                {
                    if (
                        clonedEffect.Events != Events.EnemyTurn &&
                        clonedEffect.Events != Events.Instant)
                    {
                        GameEventSystem.Instance.AddEffectToEvent(clonedEffect);
                    }
                }

                if (clonedEffect.LinkedEffect != null)
                {
                    clonedEffect.LinkedEffect.ParentEffect = clonedEffect;
                }
                clonedEffect.Actionner = permanentView.gameObject;
                clonedEffect = clonedEffect.LinkedEffect;
            }
        }
        // Si on joue une carte toute les event OnPlay ce joue (il faudrait faire des OnPlaySpell, OnPlayPermanent ect...)
        TriggerEventGA triggerEventGA = new(Events.OnPlayCard);
        ActionSystem.Instance.AddReaction(triggerEventGA);
    }

    // REACTIONS
    

    
}
