using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionSystem : Singleton<ConditionSystem>
{
    void OnEnable()
    {
        ActionSystem.AttachPerformer<TestConditionGA>(TestConditionPerformer);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<TestConditionGA>();
    }
    public IEnumerator TestConditionPerformer(TestConditionGA testConditionGA)
    {
        int Amount = 0;
        Debug.Log("fifi");
        if (testConditionGA.DynamicCondition != DynamicCondition.NULL)
        {
            switch (testConditionGA.DynamicCondition)
            {
                case DynamicCondition.NoCardsInHands:
                    if (CardSystem.Instance.hand.Count == 0)
                    {
                        ActionSystem.Instance.AddReaction(testConditionGA.EffectOnTrue.GetGameAction());
                    }
                    else
                    {
                        ActionSystem.Instance.AddReaction(testConditionGA.EffectOnFalse.GetGameAction());
                    }
                    break;

                case DynamicCondition.ValueSupOrEqualsToDynamicAmount:
                    Amount = TargetSystem.Instance.GetDynamicAmount(testConditionGA.DynamicAmount);
                    if (testConditionGA.Value >= Amount)
                    {
                        ActionSystem.Instance.AddReaction(testConditionGA.EffectOnTrue.GetGameAction());
                    }
                    else
                    {
                        ActionSystem.Instance.AddReaction(testConditionGA.EffectOnFalse.GetGameAction());
                    }
                    break;

                case DynamicCondition.ValueInfOrEqualsToDynamicAmount:
                    Amount = TargetSystem.Instance.GetDynamicAmount(testConditionGA.DynamicAmount);
                    if (testConditionGA.Value <= Amount)
                    {
                        ActionSystem.Instance.AddReaction(testConditionGA.EffectOnTrue.GetGameAction());
                    }
                    else
                    {
                        ActionSystem.Instance.AddReaction(testConditionGA.EffectOnFalse.GetGameAction());
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {

        }
        yield return null;
    }
}
