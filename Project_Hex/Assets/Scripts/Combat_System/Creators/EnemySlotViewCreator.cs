using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlotViewCreator : Singleton<EnemySlotViewCreator>
{
    public EnemySlotView SlotPrefab;
    [HideInInspector] public EnemyZoneView WeaponZone;
    [HideInInspector] public EnemyZoneView ShieldZone;
    [HideInInspector] public EnemyZoneView SupportZone;
    public EnemySlotView CreateEnemySlotViewCreator(EnemyPermanentData data, PermanentType type, bool setup = false, EnemyView enemyView = null)
    {
        GameObject Parent = null;
        switch (type)
        {
            case PermanentType.Weapon:
                Parent = WeaponZone.gameObject;
                break;
            case PermanentType.Shield:
                Parent = ShieldZone.gameObject;
                break;
            case PermanentType.Support:
                Parent = SupportZone.gameObject;
                break;
            default:
                Debug.Log("No Type For Enemy " + data.name);
                break;
        }
        if (Parent == null) return null;

        int childCount = Parent.transform.childCount;
        if (childCount >= 9)
        {
            //Debug.Log($"[EnemySlotViewCreator] Cannot add {data.name} to {type} zone — already {childCount} slots (limit = 9)");
            return null;
        }

        EnemySlotView enemySlotView = Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity, Parent.transform);
        enemySlotView.PermanentData = data;
        enemySlotView.setup();
        enemySlotView.gameObject.name = data.name + " " + CombatSystem.Instance.Enemy_Permanents.Count;

        CombatSystem.Instance.Enemy_Permanents.Add(enemySlotView);

        WeaponZone.RepositionChildrenEnemySlotView();
        ShieldZone.RepositionChildrenEnemySlotView();
        SupportZone.RepositionChildrenEnemySlotViewCenterOut();

        if (setup == true)
        {
            foreach (Effect effect in enemySlotView.PossibleIntent)
            {
                if (effect.Events == Events.Instant)
                {
                    effect.Actionner = enemySlotView.gameObject;
                    enemyView.SetupActions.Add(effect.GetGameAction());
                }
            }
        }
        else
        {
            foreach (Effect effect in enemySlotView.PossibleIntent)
            {
                if (effect.Events == Events.Instant)
                {
                    effect.Actionner = enemySlotView.gameObject;
                    ActionSystem.Instance.AddReaction(effect.GetGameAction());
                }
            }
        }



        return enemySlotView;
    }
}
