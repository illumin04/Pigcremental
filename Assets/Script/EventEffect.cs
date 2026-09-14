using UnityEngine;

// Called once at occurrence. Return the ACTUAL result for the notification.
// Empty/null string means an invisible effect. Asset fields are configuration,
// not per-game mutable state; store that state on your game systems instead.
public abstract class EventEffect : ScriptableObject
{
    public abstract string Execute(GameManager game);
}
