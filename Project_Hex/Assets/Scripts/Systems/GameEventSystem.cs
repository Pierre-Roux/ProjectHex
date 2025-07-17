using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameEventSystem : Singleton<GameEventSystem>
{
    private Dictionary<Events, List<Effect>> effectsByEvent = new();

    void OnEnable()
    {
        ActionSystem.AttachPerformer<TriggerEventGA>(TriggerEvent);
        ActionSystem.AttachPerformer<TriggerPermanentEventGA>(TriggerPermanentEventPerformer);
        ActionSystem.AttachPerformer<TriggerEnemyEventGA>(TriggerEnemyEventPerformer);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<TriggerEventGA>();
        ActionSystem.DetachPerformer<TriggerPermanentEventGA>();
        ActionSystem.DetachPerformer<TriggerEnemyEventGA>();
    }



    //PERFORMERS

    public void AddEffectToEvent(Effect effectToExecute)
    {
        if (!effectsByEvent.TryGetValue(effectToExecute.Events, out var list))
        {
            list = new List<Effect>();
            effectsByEvent[effectToExecute.Events] = list;
        }
        list.Add(effectToExecute);
    }

    public IEnumerator TriggerEvent(TriggerEventGA triggerEventGA)
    {
        if (effectsByEvent.TryGetValue(triggerEventGA.gameEvent, out var list))
        {
            foreach (var effect in list)
            {
                if (triggerEventGA.card == null)
                {
                    GameAction ga = effect.GetGameAction();
                    if (ga != null)
                        ActionSystem.Instance.AddReaction(ga);
                }
                else
                {
                    if (triggerEventGA.card == effect.CardActionner)
                    {
                        GameAction ga = effect.GetGameAction();
                        if (ga != null)
                            ActionSystem.Instance.AddReaction(ga);
                    }
                }  
            }

            /*List<Effect> EffectToRemove = new List<Effect>();
            EffectToRemove.Add(effect);
            for (int i = 0; i < EffectToRemove.Count - 1; i++)
            {
                RemoveEffect(EffectToRemove[i]);
            }*/

            yield return null;
        }
    }

    public void ClearAllEvents()
    {
        effectsByEvent.Clear();
    }

    public void RemoveEffect(Effect effect)
    {
        if (effectsByEvent.TryGetValue(effect.Events, out var list))
        {
            list.Remove(effect);
        }
    }

    public void RemoveEffectByActionner(GameObject GOToSuppr)
    {
        GameObject actionnerToRemove = GOToSuppr;
        var eventsToCleanUp = new List<Events>();

        foreach (var eventEntry in effectsByEvent)
        {
            Events gameEvent = eventEntry.Key;
            List<Effect> effectList = eventEntry.Value;

            for (int i = effectList.Count - 1; i >= 0; i--)
            {
                if (effectList[i].Actionner == actionnerToRemove)
                {
                    effectList.RemoveAt(i);
                }
            }

            if (effectList.Count == 0)
            {
                eventsToCleanUp.Add(gameEvent);
            }
        }

        // Nettoyer les événements devenus vides
        foreach (var gameEvent in eventsToCleanUp)
        {
            effectsByEvent.Remove(gameEvent);
        }
    }

    public IEnumerator TriggerPermanentEventPerformer(TriggerPermanentEventGA triggerPermanentEventGA)
    {
        //Debug.Log("TriggerPermanentEvent : " + triggerPermanentEventGA.gameEvent + " at " + Time.timeSinceLevelLoad);

        if (!effectsByEvent.TryGetValue(triggerPermanentEventGA.gameEvent, out var effectList))
            yield break;

        if (triggerPermanentEventGA.permanentView != null)
        {
            foreach (var effect in effectList)
            {
                if (effect.Actionner.GetComponent<PermanentView>() == triggerPermanentEventGA.permanentView)
                {
                    GameAction ga = effect.GetGameAction();
                    if (ga != null)
                        ActionSystem.Instance.AddReaction(ga);
                }
                yield return null;
            }
        }
    }

    public IEnumerator TriggerEnemyEventPerformer(TriggerEnemyEventGA triggerEnemyEventGA)
    {
        //Debug.Log("TriggerEnnemy : " + triggerEnemyEventGA.gameEvent + " at " + Time.timeSinceLevelLoad);

        if (!effectsByEvent.TryGetValue(triggerEnemyEventGA.gameEvent, out var effectList))
            yield break;

        if (triggerEnemyEventGA.enemySlotView != null)
        {
            foreach (var effect in effectList)
            {
                if (effect.Actionner.GetComponent<EnemySlotView>() == triggerEnemyEventGA.enemySlotView)
                {
                    GameAction ga = effect.GetGameAction();
                    if (ga != null)
                        ActionSystem.Instance.AddReaction(ga);
                }
                yield return null;
            }
        }
    }
}
