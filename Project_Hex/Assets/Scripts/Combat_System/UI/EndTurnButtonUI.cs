using Unity.VisualScripting;
using UnityEngine;

public class EndTurnButtonUI : MonoBehaviour
{

    public void OnClick()
    {
        if (!CombatSystem.Instance.Interactable) return;
        EnemyTurnGA enemyTurnGA = new();
        ActionSystem.Instance.Perform(enemyTurnGA);
    }

}
