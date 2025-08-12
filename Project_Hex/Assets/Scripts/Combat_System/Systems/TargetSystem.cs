using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class TargetSystem : Singleton<TargetSystem>
{
    [SerializeField] private LayerMask TargetingLayerMask;
    [SerializeField] private GameObject CursorGameobject;
    private bool TargetingActive;
    private int InitTargetingNumber;
    private int TargetingNumber;
    private List<EnemySlotView> enemySlots = new();
    private List<PermanentView> permanents = new();

    public void OnEnable()
    {
        ActionSystem.AttachPerformer<StartManualTargetingGA>(GetTargetsManualPerformer);
        
    }

    public void OnDisable()
    {
        ActionSystem.DetachPerformer<StartManualTargetingGA>();
    }

    public IEnumerator GetTargetsManualPerformer(StartManualTargetingGA startManualTargetingGA)
    {
        List<PermanentView> playerTargets = new();
        List<EnemySlotView> enemyTargets = new();
        TargetingNumber = InitTargetingNumber = startManualTargetingGA.TargetNumber;

        StartManualTargeting();

        while (TargetingActive)
            yield return null;

        (enemyTargets, playerTargets) = EndManualTargeting();

        var action = startManualTargetingGA.ActionToRealiseAfterTargetting;
        var type = action.GetType();

        // Vérifie qu'il y a bien les propriétés attendues
        var playerTargetsProp = type.GetProperty("Targets_Player");
        var enemyTargetsProp = type.GetProperty("Targets_Enemy");

        if (playerTargetsProp != null && enemyTargetsProp != null)
        {
            playerTargetsProp.SetValue(action, playerTargets);
            enemyTargetsProp.SetValue(action, enemyTargets);
        }
        else
        {
            Debug.LogError("L'action ne contient pas les propriétés Targets_Player ou Targets_Enemy");
        }

        ActionSystem.Instance.AddReaction(startManualTargetingGA.ActionToRealiseAfterTargetting);
    }
    
    public static (List<PermanentView> playerTargets, List<EnemySlotView> enemyTargets) GetTargets(TargetMode mode, GameObject actionner)
    {
        List<PermanentView> playerTargets = new();
        List<EnemySlotView> enemyTargets = new();

        var playerPermanents = CombatSystem.Instance.Player_Permanents;
        var enemyPermanents = CombatSystem.Instance.Enemy_Permanents;
        
        List<PermanentView> TampontargetsP = new List<PermanentView>();
        List<EnemySlotView> TampontargetsE = new List<EnemySlotView>();

        switch (mode)
        {
            case TargetMode.Self:
                PermanentView TestIfPlayerPermanent = actionner.GetComponent<PermanentView>();
                if (TestIfPlayerPermanent)
                {
                    var self = actionner.GetComponent<PermanentView>();
                    if (self != null)
                        playerTargets.Add(self);
                }
                else
                {
                    var self = actionner.GetComponent<EnemySlotView>();
                    if (self != null)
                        enemyTargets.Add(self);
                }
                break;

            case TargetMode.Random_Player:
                var targetablePlayers = playerPermanents
                    .Where(p => p.Targetable)
                    .ToList();

                if (targetablePlayers.Count > 0)
                {
                    var rnd = Random.Range(0, targetablePlayers.Count);
                    playerTargets.Add(targetablePlayers[rnd]);
                }
                break;

            case TargetMode.Core_Player:
                foreach (var perm in playerPermanents)
                    if (perm.IsCore && perm.Targetable) playerTargets.Add(perm);
                break;

            case TargetMode.HighHP_Player:
                int maxTotal = playerPermanents.Max(p => p.currentLife);
                var highestTargets = playerPermanents
                    .Where(p => p.currentLife == maxTotal && p.Targetable)
                    .ToList();

                if (highestTargets.Count > 0)
                {
                    var selected = highestTargets[Random.Range(0, highestTargets.Count)];
                    playerTargets.Add(selected);
                }
                break;

            case TargetMode.LowHP_Player:
                int minTotal = playerPermanents.Min(p => p.currentLife);
                var lowestTargets = playerPermanents
                    .Where(p => p.currentLife == minTotal && p.Targetable)
                    .ToList();

                if (lowestTargets.Count > 0)
                {
                    var selected = lowestTargets[Random.Range(0, lowestTargets.Count)];
                    playerTargets.Add(selected);
                }
                break;

            case TargetMode.Random_Enemy:
                var targetableEnemies = enemyPermanents
                    .Where(p => p.Targetable)
                    .ToList();

                if (playerPermanents.Count > 0 && targetableEnemies.Count > 0)
                {
                    var rnd = Random.Range(0, targetableEnemies.Count);
                    enemyTargets.Add(targetableEnemies[rnd]);
                }
                break;

            case TargetMode.Core_Enemy:
                foreach (var perm in enemyPermanents)
                {
                    if (perm.IsCore && perm.Targetable)
                    {
                        enemyTargets.Add(perm);
                    }   
                }
                break;

            case TargetMode.HighHP_Enemy:
                int maxTotal2 = enemyPermanents.Max(p => p.currentLife);
                var highestTargets2 = enemyPermanents
                    .Where(p => p.currentLife == maxTotal2 && p.Targetable)
                    .ToList();

                if (highestTargets2.Count > 0)
                {
                    var selected = highestTargets2[Random.Range(0, highestTargets2.Count)];
                    enemyTargets.Add(selected);
                }
                break;

            case TargetMode.LowHP_Enemy:
                int minTotal2 = enemyPermanents.Min(p => p.currentLife);
                var lowestTargets2 = enemyPermanents
                    .Where(p => p.currentLife == minTotal2 && p.Targetable)
                    .ToList();

                if (lowestTargets2.Count > 0)
                {
                    var selected = lowestTargets2[Random.Range(0, lowestTargets2.Count)];
                    enemyTargets.Add(selected);
                }
                break;

            case TargetMode.All_Player:
                foreach (var perm in playerPermanents)
                {
                    if(!perm.Targetable) continue;
                    playerTargets.Add(perm);

                }
                break;

            case TargetMode.All_Enemy:
                foreach (var perm in enemyPermanents)
                {
                    if(!perm.Targetable) continue;
                    enemyTargets.Add(perm);

                }
                break;

            case TargetMode.All_All:
                foreach (var perm in playerPermanents)
                {
                    if(!perm.Targetable) continue;
                    playerTargets.Add(perm);

                }
                foreach (var perm in enemyPermanents)
                {
                    if(!perm.Targetable) continue;
                    enemyTargets.Add(perm);

                }
                break;
            case TargetMode.ALL_Player_Weapons:
                foreach (var perm in playerPermanents)
                {
                    if(!perm.Targetable) continue;
                    if (perm.permanentType == PermanentType.Weapon)
                    {
                        playerTargets.Add(perm);
                    }
                }
                break;
            case TargetMode.ALL_Player_Shields:
                foreach (var perm in playerPermanents)
                {
                    if(!perm.Targetable) continue;
                    if (perm.permanentType == PermanentType.Shield)
                    {
                        playerTargets.Add(perm);
                    }
                }
                break;
            case TargetMode.ALL_Player_Supports:
                foreach (var perm in playerPermanents)
                {
                    if(!perm.Targetable) continue;
                    if (perm.permanentType == PermanentType.Support)
                    {
                        playerTargets.Add(perm);
                    }
                }
                break;
            case TargetMode.ALL_Enemy_Weapons:
                foreach (var perm in enemyPermanents)
                {
                    if(!perm.Targetable) continue;
                    if (perm.permanentType == PermanentType.Weapon)
                    {
                        enemyTargets.Add(perm);
                    }
                }
                break;
            case TargetMode.ALL_Enemy_Shields:
                foreach (var perm in enemyPermanents)
                {
                    if(!perm.Targetable) continue;
                    if (perm.permanentType == PermanentType.Shield)
                    {
                        enemyTargets.Add(perm);
                    }
                }
                break;
            case TargetMode.ALL_Enemy_Supports:
                foreach (var perm in enemyPermanents)
                {
                    if(!perm.Targetable) continue;
                    if (perm.permanentType == PermanentType.Support)
                    {
                        enemyTargets.Add(perm);
                    }
                }
                break;
            case TargetMode.RDM_Player_Weapons:
                TampontargetsP = new List<PermanentView>();
                foreach (var perm in playerPermanents)
                {
                    if(!perm.Targetable) continue;
                    if (perm.permanentType == PermanentType.Weapon)
                    {
                        TampontargetsP.Add(perm);
                    }
                }

                playerTargets.Add(TampontargetsP[Random.Range(0, TampontargetsP.Count - 1)]);

                break;
            case TargetMode.RDM_Player_Shields:
                TampontargetsP = new List<PermanentView>();
                foreach (var perm in playerPermanents)
                {
                    if(!perm.Targetable) continue;
                    if (perm.permanentType == PermanentType.Shield)
                    {
                        TampontargetsP.Add(perm);
                    }
                }

                playerTargets.Add(TampontargetsP[Random.Range(0, TampontargetsP.Count - 1)]);

                break;
            case TargetMode.RDM_Player_Supports:
                TampontargetsP = new List<PermanentView>();
                foreach (var perm in playerPermanents)
                {
                    if(!perm.Targetable) continue;
                    if (perm.permanentType == PermanentType.Support)
                    {
                        TampontargetsP.Add(perm);
                    }
                }

                playerTargets.Add(TampontargetsP[Random.Range(0, TampontargetsP.Count - 1)]);

                break;
            case TargetMode.RDM_Enemy_Weapons:
                TampontargetsE = new List<EnemySlotView>();
                foreach (var perm in enemyPermanents)
                {
                    if(!perm.Targetable) continue;
                    if (perm.permanentType == PermanentType.Weapon)
                    {
                        TampontargetsE.Add(perm);
                    }
                }

                enemyTargets.Add(TampontargetsE[Random.Range(0, TampontargetsE.Count - 1)]);

                break;
            case TargetMode.RDM_Enemy_Shields:
                TampontargetsE = new List<EnemySlotView>();
                foreach (var perm in enemyPermanents)
                {
                    if(!perm.Targetable) continue;
                    if (perm.permanentType == PermanentType.Shield)
                    {
                        TampontargetsE.Add(perm);
                    }
                }

                enemyTargets.Add(TampontargetsE[Random.Range(0, TampontargetsE.Count - 1)]);

                break;
            case TargetMode.RDM_Enemy_Supports:
                TampontargetsE = new List<EnemySlotView>();
                foreach (var perm in enemyPermanents)
                {
                    if(!perm.Targetable) continue;
                    if (perm.permanentType == PermanentType.Support)
                    {
                        TampontargetsE.Add(perm);
                    }
                }

                enemyTargets.Add(TampontargetsE[Random.Range(0, TampontargetsE.Count - 1)]);

                break;
        }

        return (playerTargets, enemyTargets);
    }

    public void StartManualTargeting()
    {
        enemySlots.Clear();
        permanents.Clear();
        TargetingActive = true;
    }

    public (List<EnemySlotView> enemyTargets, List<PermanentView> playerTargets) EndManualTargeting()
    {
        TargetingActive = false;
        return (enemySlots, permanents);
    }

    public void Update()
    {
        if (TargetingActive)
        {
            if (Input.GetKeyDown(KeyCode.Space)) // Espace = confirmer
            {
                TargetingActive = false;
                foreach (EnemySlotView enemy in enemySlots)
                {
                    enemy.RemoveSelectEffect();
                }
                foreach (PermanentView permanent in permanents)
                {
                    permanent.RemoveSelectEffect();
                }
            }
            if (Input.GetMouseButtonDown(0)) // 0 = clic gauche 1 = clic droit
            {
                Debug.DrawRay(CursorGameobject.transform.position + new Vector3(0, 0, -1), Vector3.forward * 10f, Color.red, 1f);
                if (Physics.Raycast(CursorGameobject.transform.position + new Vector3(0, 0, -1), Vector3.forward, out RaycastHit raycastHit, 10f, TargetingLayerMask) && raycastHit.collider != null && raycastHit.transform.TryGetComponent(out EnemySlotView enemyView))
                {
                    if (enemyView.Targetable)
                    {
                        if (!enemySlots.Contains(enemyView))
                        {
                            if (TargetingNumber > 0)
                            {
                                enemySlots.Add(enemyView);
                                enemyView.ActiveSelectEffect();
                                TargetingNumber -= 1;
                            }
                        }
                        else
                        {
                            if (TargetingNumber < InitTargetingNumber)
                            {
                                enemySlots.Remove(enemyView);
                                enemyView.RemoveSelectEffect();
                                TargetingNumber += 1;
                            }
                        }
                    }
                }
                else if (Physics.Raycast(CursorGameobject.transform.position + new Vector3(0, 0, -1), Vector3.forward, out RaycastHit raycastHit2, 10f, TargetingLayerMask) && raycastHit2.collider != null && raycastHit2.transform.TryGetComponent(out PermanentView permanentView))
                {
                    if (permanentView.Targetable)
                    {
                        if (!permanents.Contains(permanentView))
                        {
                            if (TargetingNumber > 0)
                            {
                                permanents.Add(permanentView);
                                permanentView.ActiveSelectEffect();
                                TargetingNumber -= 1;
                            }
                        }
                        else
                        {
                            if (TargetingNumber < InitTargetingNumber)
                            {
                                permanents.Remove(permanentView);
                                permanentView.RemoveSelectEffect();
                                TargetingNumber += 1;
                            }
                        }
                    }
                }
            }
        }
    }
}
