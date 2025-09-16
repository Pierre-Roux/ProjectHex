using System.Collections;
using System.Collections.Generic;
using FMODUnity;
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
        if (childCount >= CombatSystem.Instance.MaxPermEnemy)
        {
            //Debug.Log($"[EnemySlotViewCreator] Cannot add {data.name} to {type} zone — already {childCount} slots (limit = 9)");
            return null;
        }

        if (!AudioManager.Instance.IsValid(data.SummonEPermanentSound))
        {
            RuntimeManager.PlayOneShot(AudioManager.Instance.SummonEPermanentSound);
        }
        else
        {
            RuntimeManager.PlayOneShot(data.SummonEPermanentSound);
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
                Effect clonedEffect = effect.Clone();

                while (clonedEffect != null)
                {
                    if (clonedEffect.Events == Events.Instant)
                    {
                        clonedEffect.Actionner = enemySlotView.gameObject;
                        enemyView.SetupActions.Add(clonedEffect.GetGameAction());
                    }

                    if (clonedEffect.LinkedEffect != null)
                    {
                        clonedEffect.LinkedEffect.ParentEffect = clonedEffect;
                    }
                    clonedEffect.Actionner = enemySlotView.gameObject;
                    clonedEffect = clonedEffect.LinkedEffect;
                }
            }
        }
        else
        {
            foreach (Effect effect in enemySlotView.PossibleIntent)
            {

                Effect clonedEffect = effect.Clone();
                
                while (clonedEffect != null)
                {
                    if (clonedEffect.Events == Events.Instant)
                    {
                        clonedEffect.Actionner = enemySlotView.gameObject;
                        ActionSystem.Instance.AddReaction(clonedEffect.GetGameAction());
                    }
                    else
                    {
                        if (clonedEffect.Events != Events.EnemyTurn &&
                            clonedEffect.Events != Events.Instant)
                        {
                            GameEventSystem.Instance.AddEffectToEvent(clonedEffect);
                        }
                    }
                    if (clonedEffect.LinkedEffect != null)
                    {
                        clonedEffect.LinkedEffect.ParentEffect = clonedEffect;
                    }
                    clonedEffect.Actionner = enemySlotView.gameObject;
                    clonedEffect = clonedEffect.LinkedEffect;
                }
            }
        }



        return enemySlotView;
    }
}
