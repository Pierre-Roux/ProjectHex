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
                if (playerPermanents.Count > 0)
                {
                    var rnd = Random.Range(0, playerPermanents.Count);
                    playerTargets.Add(playerPermanents[rnd]);
                }
                break;

            case TargetMode.Core_Player:
                foreach (var perm in playerPermanents)
                    if (perm.IsCore) playerTargets.Add(perm);
                break;

            case TargetMode.HighHP_Player:
                int maxTotal = playerPermanents.Max(p => p.currentLife + p.currentShield);
                var highestTargets = playerPermanents
                    .Where(p => p.currentLife + p.currentShield == maxTotal)
                    .ToList();

                if (highestTargets.Count > 0)
                {
                    var selected = highestTargets[Random.Range(0, highestTargets.Count)];
                    playerTargets.Add(selected);
                }
                break;

            case TargetMode.LowHP_Player:
                int minTotal = playerPermanents.Min(p => p.currentLife + p.currentShield);
                var lowestTargets = playerPermanents
                    .Where(p => p.currentLife + p.currentShield == minTotal)
                    .ToList();

                if (lowestTargets.Count > 0)
                {
                    var selected = lowestTargets[Random.Range(0, lowestTargets.Count)];
                    playerTargets.Add(selected);
                }
                break;

            case TargetMode.Random_Enemy:
                if (playerPermanents.Count > 0)
                {
                    var rnd = Random.Range(0, enemyPermanents.Count);
                    enemyTargets.Add(enemyPermanents[rnd]);
                }
                break;

            case TargetMode.Core_Enemy:
                foreach (var perm in enemyPermanents)
                    if (perm.IsCore) enemyTargets.Add(perm);
                break;

            case TargetMode.HighHP_Enemy:
                int maxTotal2 = enemyPermanents.Max(p => p.currentLife + p.currentShield);
                var highestTargets2 = enemyPermanents
                    .Where(p => p.currentLife + p.currentShield == maxTotal2)
                    .ToList();

                if (highestTargets2.Count > 0)
                {
                    var selected = highestTargets2[Random.Range(0, highestTargets2.Count)];
                    enemyTargets.Add(selected);
                }
                break;

            case TargetMode.LowHP_Enemy:
                int minTotal2 = enemyPermanents.Min(p => p.currentLife + p.currentShield);
                var lowestTargets2 = enemyPermanents
                    .Where(p => p.currentLife + p.currentShield == minTotal2)
                    .ToList();

                if (lowestTargets2.Count > 0)
                {
                    var selected = lowestTargets2[Random.Range(0, lowestTargets2.Count)];
                    enemyTargets.Add(selected);
                }
                break;

            case TargetMode.All_Player:
                playerTargets.AddRange(playerPermanents);
                break;

            case TargetMode.All_Enemy:
                enemyTargets.AddRange(enemyPermanents);
                break;

            case TargetMode.All_All:
                playerTargets.AddRange(playerPermanents);
                enemyTargets.AddRange(enemyPermanents);
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
                else if (Physics.Raycast(CursorGameobject.transform.position + new Vector3(0, 0, -1), Vector3.forward, out RaycastHit raycastHit2, 10f, TargetingLayerMask) && raycastHit2.collider != null && raycastHit2.transform.TryGetComponent(out PermanentView permanentView))
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
