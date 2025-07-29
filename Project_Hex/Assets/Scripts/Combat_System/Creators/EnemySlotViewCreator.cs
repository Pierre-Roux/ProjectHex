using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlotViewCreator : Singleton<EnemySlotViewCreator>
{
    public EnemySlotView SlotPrefab;
    [HideInInspector] public EnemyZoneView WeaponZone;
    [HideInInspector] public EnemyZoneView ShieldZone;
    [HideInInspector] public EnemyZoneView SupportZone;
    public EnemySlotView CreateEnemySlotViewCreator(EnemyPermanentData data, PermanentType type)
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
        EnemySlotView enemySlotView = Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity, Parent.transform);
        enemySlotView.PermanentData = data;
        enemySlotView.setup();
        enemySlotView.gameObject.name = data.name + " " + CombatSystem.Instance.Enemy_Permanents.Count;

        CombatSystem.Instance.Enemy_Permanents.Add(enemySlotView);

        WeaponZone.RepositionChildrenEnemySlotView();
        ShieldZone.RepositionChildrenEnemySlotView();
        SupportZone.RepositionChildrenEnemySlotViewCenterOut();

        return enemySlotView;
    }
}
