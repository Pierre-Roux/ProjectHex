public enum Events
{
    Instant,

    // General Event
    StartTurn,
    EndTurn,
    OnPlay,
    OnDiscard,
    OnDraw,

    //Permanent or EnemyPermanent Event
    OnDeath,
    OnActivate,
    OnDamaged,
    OnDestroy,

    //Card Event (ON DrawThis, onDiscardThis ...)

}