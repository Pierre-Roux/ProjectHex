using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBase : Singleton<DataBase>
{
    public List<CardData> GlobalCardList;
    
    public int Money;
    public int MaxMana;
    public PlayerData StartingPlayer;
    public List<GameObject> EnemiesDataBase;

    [HideInInspector] public List<CardData> INITIALDeckList;
    [HideInInspector] public List<CardData> DeckList;
    [HideInInspector] public int CurrentStage;

    //For fight
    public bool IsElite;
    public int CoreLife;

    public new void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
}
