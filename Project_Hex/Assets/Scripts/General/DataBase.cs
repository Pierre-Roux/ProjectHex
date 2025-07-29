using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBase : Singleton<DataBase>
{
    public List<CardData> INITIALDeckList;
    public List<CardData> DeckList;
    public int CurrentStage;
    public List<CardData> GlobalCardList;
    public int Money;

    //For fight
    public bool IsElite;
    public int CoreLife;

    public new void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
}
