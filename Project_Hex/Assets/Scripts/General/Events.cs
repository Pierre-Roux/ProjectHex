public enum Events
{
    Instant,

    // General Event
    EnemyTurn,
    EndEnemyTurn,
    StartTurn,
    EndTurn,
    OnPlayCard,
    OnPlaySpell,
    OnPlayPerma,
    OnInvoc,
    OnDiscard,
    OnDraw,

    //Permanent or EnemyPermanent Event
    OnDeath,
    OnSacrifice,
    OnActivate,
    OnDamaged,
    OnDestroy,
    OnKill,
    WhenPermaDie,
    WhenPermaSac,
    WhenPermaExaust,
    WhenPermaBecomeType,

    //Card Event (ON DrawThis, onDiscardThis ...)
    Null,
}