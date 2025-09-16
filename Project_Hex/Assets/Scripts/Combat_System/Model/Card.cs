using System.Collections.Generic;
using FMODUnity;
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
    public bool isInvoc;

    //Audio
    public EventReference PlayCardSound;
    public EventReference DiscardCardSound;
    public EventReference DrawCardSound;
    public EventReference SummonPPermanentSound;
    public EventReference BeingDamageSound;
    public EventReference DieSound;
    public EventReference HollowDieSound;
    public EventReference BeingHealSound;
    public EventReference BeingShieldSound;
    public EventReference LoseShieldSound;
    public EventReference GainPowerSound;
    public EventReference LosePowerSound;
    public EventReference TakeLifeLossSound;
    public EventReference BuffLifeSound;
    public EventReference DebuffLifeSound;
    public EventReference SelectedSound;
    public EventReference UnSelectedSound;

    public List<Effect> Effects => data.Effects;


    public Card(CardData cardData)
    {
        data = cardData;
        cost = cardData.cost;
        IsSpell = cardData.IsSpell;
        Money_Cost = data.Money_Cost;
        isInvoc = data.isInvoc;
        if (!cardData.IsSpell)
        {
            life = cardData.life;
            Durability = cardData.Durability;
            DecayCounter = cardData.DecayCounter;
            MaxDurability = cardData.MaxDurability;
        }

        if (cardData.PlayCardSound.Path != "") PlayCardSound = cardData.PlayCardSound;
        if (cardData.DiscardCardSound.Path != "") DiscardCardSound = cardData.DiscardCardSound;
        if (cardData.DrawCardSound.Path != "") DrawCardSound = cardData.DrawCardSound;
        if (cardData.SummonPPermanentSound.Path != "") SummonPPermanentSound = cardData.SummonPPermanentSound;
        if (cardData.BeingDamageSound.Path != "") BeingDamageSound = cardData.BeingDamageSound;
        if (cardData.DieSound.Path != "") DieSound = cardData.DieSound;
        if (cardData.HollowDieSound.Path != "") HollowDieSound = cardData.HollowDieSound;
        if (cardData.BeingHealSound.Path != "") BeingHealSound = cardData.BeingHealSound;
        if (cardData.BeingShieldSound.Path != "") BeingShieldSound = cardData.BeingShieldSound;
        if (cardData.LoseShieldSound.Path != "") LoseShieldSound = cardData.LoseShieldSound;
        if (cardData.GainPowerSound.Path != "") GainPowerSound = cardData.GainPowerSound;
        if (cardData.LosePowerSound.Path != "") LosePowerSound = cardData.LosePowerSound;
        if (cardData.TakeLifeLossSound.Path != "") TakeLifeLossSound = cardData.TakeLifeLossSound;
        if (cardData.BuffLifeSound.Path != "") BuffLifeSound = cardData.BuffLifeSound;
        if (cardData.DebuffLifeSound.Path != "") DebuffLifeSound = cardData.DebuffLifeSound;
        if (cardData.SelectedSound.Path != "") SelectedSound = cardData.SelectedSound;
        if (cardData.UnSelectedSound.Path != "") UnSelectedSound = cardData.UnSelectedSound;
    }
}
