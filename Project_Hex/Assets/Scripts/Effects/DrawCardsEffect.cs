using UnityEngine;

public class DrawCardsEffect : Effect
{

    [SerializeField] private int drawAmount;

    public override GameAction GetGameAction()
    {
        DrawCardsGA drawCardsGA = new(drawAmount);
        return drawCardsGA;
    }
    public DrawCardsEffect(){}

    public DrawCardsEffect(int Amount, ActionnerType ActionnerType, Events Event, GameObject actionner, Card cardActionner)
    {
        drawAmount = Amount;
        Events = Event;
        actionnerType = ActionnerType;
        Actionner = actionner;
        CardActionner = cardActionner;
    }
    public override Effect Clone()
    {
        return new DrawCardsEffect(drawAmount, actionnerType ,Events,Actionner,CardActionner);
    }

}
