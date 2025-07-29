using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventSelectorSystem : Singleton<EventSelectorSystem>
{

    [SerializeField] public List<Choice> choices;


    public void Start()
    {
        ShuffleChoice();
    }

    public void ShuffleChoice()
    {
        int CurrentStage = DataBase.Instance.CurrentStage;
        foreach (Choice choice in choices)
        {
            choice.isElite = new System.Random().Next(0, 2) == 0;
            if (choice.isElite)
            {
                choice.UpdateText("Stage " + CurrentStage + " Enemy", "ELITE");
            }
            else
            {
                choice.UpdateText("Stage " + CurrentStage + " Enemy", "");
            }

        }
    }

    public void StartFight(int index)
    {
        DataBase.Instance.IsElite = choices[index-1].isElite;

        SceneManager.LoadScene("CombatScene");
    }
}
