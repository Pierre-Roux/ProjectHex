public enum TargetLimitations
{
    NULL,

    OnlyPlayerPermanent,
    OnlyEnemyPermanent,
    OnlyTypePermanent,
    
    CardCostValue,
    CardCostMoreThanValue,
    CardCostLessThanValue,
    
}

[System.Serializable]
public class TargetLimitationInfo
{
    public TargetLimitations targetLimitations;
    public PermaTypes PermaType;
    public int IntValue = -1;

    public TargetLimitationInfo(){}

    public TargetLimitationInfo(int intValue, PermaTypes permaType, TargetLimitations TargetLimitations)
    {
        IntValue = intValue;
        PermaType = permaType;
        targetLimitations = TargetLimitations;
    }
}

