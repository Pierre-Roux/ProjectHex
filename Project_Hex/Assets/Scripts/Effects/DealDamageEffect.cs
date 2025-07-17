using DG.Tweening;
using UnityEngine;

public class DealDamageEffect : Effect
{
    [SerializeField] private int damageAmount;
    [SerializeField] private TargetMode targetMode;
    [SerializeField] private int targetNumber;

    public DealDamageEffect(){}

    public DealDamageEffect(int DamageAmount, TargetMode TargetMode, int TargetNumber, ActionnerType ActionnerType, Events Event, GameObject actionner, Card cardActionner)
    {
        damageAmount = DamageAmount;
        targetMode = TargetMode;
        targetNumber = TargetNumber;
        actionnerType = ActionnerType;
        Events = Event;
        Actionner = actionner;
        CardActionner = cardActionner;
    }

    public override GameAction GetGameAction()
    {
        Debug.Log("Actionner : " + Actionner);
        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (targetMode == TargetMode.Manual)
            {
                DealDamageGA dealDamageGA = new(damageAmount, null, null);
                StartManualTargetingGA startManualTargetingGA = new(dealDamageGA, targetNumber);
                return startManualTargetingGA;
            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetMode, null);
                DealDamageGA dealDamageGA = new(damageAmount, playerTargets, enemyTargets);
                return dealDamageGA;
            }
        }
        else
        {
            if (actionnerType == ActionnerType.ENEMY && Actionner != null)
            {
                _AttackPlayerGA _AttackPlayerGA = new(damageAmount, targetMode);
                _AttackPlayerGA.Actionner = Actionner;
                return _AttackPlayerGA;
            }
            else if (actionnerType == ActionnerType.PLAYER && Actionner != null)
            {
                Debug.Log("Starting An Attack From Player " + Actionner.name);
                AttackEnemyGA attackEnemyGA = new(damageAmount, targetMode);
                attackEnemyGA.Actionner = Actionner;
                return attackEnemyGA;
            }
            else
            {
                Debug.Log("Effect.GetGameAction returned Null");
                return null;
            }
        }

    }

    public override Effect Clone()
    {
        return new DealDamageEffect(damageAmount, targetMode, targetNumber, actionnerType, Events, Actionner,CardActionner);
    }
}
