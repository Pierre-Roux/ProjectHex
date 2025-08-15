using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Card
{
    public readonly CardData data;

    public string Title => data.Title;
    public string Description => data.Description;
    public Sprite Image => data.Image;
    public PermanentType permanentType => data.permanentType;
    public bool UnShieldable => data.UnShieldable;
    

    public bool IsSpell { get; private set; }
    public int cost { get; private set; }
    public int life { get; private set; }
    public int Shield { get; private set; }
    public int Durability { get; set; }
    public int DecayCounter { get; set; }
    public int MaxDurability { get; set; }
    public int Money_Cost { get; set; }

    public List<Effect> Effects => data.Effects;


    public Card(CardData cardData)
    {
        data = cardData;
        cost = cardData.cost;
        IsSpell = cardData.IsSpell;
        Money_Cost = data.Money_Cost;
        if (!cardData.IsSpell)
        {
            life = cardData.life;
            Durability = cardData.Durability;
            DecayCounter = cardData.DecayCounter;
            MaxDurability = cardData.MaxDurability;
        }
    }
}
