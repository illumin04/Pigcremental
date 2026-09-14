using UnityEngine;

// A reusable condition asset. Query state only; never mutate or roll random here.
public abstract class EventCondition : ScriptableObject
{
    public abstract bool IsMet(GameManager game);
}
