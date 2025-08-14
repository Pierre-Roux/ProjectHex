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
    public int damage { get; private set; }
    public int Shield { get; private set; }
    public int Durability { get; set; }
    public int Money_Cost => data.Money_Cost;

    public List<Effect> Effects => data.Effects;


    public Card(CardData cardData)
    {
        data = cardData;
        cost = cardData.cost;
        IsSpell = cardData.IsSpell;
        if (!cardData.IsSpell)
        {
            life = cardData.life;
            damage = cardData.damage;
            Durability = cardData.Durability;
        }
    }
}
