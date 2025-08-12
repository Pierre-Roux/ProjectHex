using UnityEngine;
using System;

public class DrawCardsEffect : Effect
{
    [Header("Effect Param")]
    [SerializeField] private int drawAmount;

    public override GameAction GetGameAction()
    {
        DrawCardsGA drawCardsGA = new(drawAmount);
        return drawCardsGA;
    }
    public DrawCardsEffect(){}

    public DrawCardsEffect(int Amount, ActionnerType ActionnerType, Events Event, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType)
    {
        drawAmount = Amount;
        Events = Event;
        actionnerType = ActionnerType;
        Actionner = actionner;
        CardActionner = cardActionner;
        Intent_Title = intent_Title;
        number = Number;
        Duration = duration;
        DurationType = durationType;
    }
    public override Effect Clone()
    {
        return new DrawCardsEffect(drawAmount, actionnerType ,Events,Actionner,CardActionner,Intent_Title,number,Duration,DurationType);
    }

}
