public enum Events
{
    Instant,

    // General Event
    EnemyTurn,
    EndEnemyTurn,
    StartTurn,
    EndTurn,
    OnPlayCard,
    OnDiscard,
    OnDraw,

    //Permanent or EnemyPermanent Event
    OnDeath,
    OnActivate,
    OnDamaged,
    OnDestroy,
    OnKill,
    WhenPermaDie,

    //Card Event (ON DrawThis, onDiscardThis ...)
    Null,
}