using JetBrains.Annotations;
using UnityEngine;

public class TestConditionGA : GameAction
{
    public DynamicCondition DynamicCondition;
    public Effect EffectOnTrue;
    public Effect EffectOnFalse;
    public int Value;
    public DynamicAmount DynamicAmount;
    public TestConditionGA(DynamicCondition dynamicCondition, Effect effectOnTrue, Effect effectOnFalse, int value = 0, DynamicAmount dynamicAmount = DynamicAmount.NULL)
    {
        DynamicCondition = dynamicCondition;
        EffectOnTrue = effectOnTrue;
        EffectOnFalse = effectOnFalse;
        Value = value;
        DynamicAmount = dynamicAmount;
    }
}
