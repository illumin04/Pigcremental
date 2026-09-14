using UnityEngine;

[CreateAssetMenu(
    fileName = "NewEvent",
    menuName = "Pigcremental/Event"
)]
public class EventData : ScriptableObject
{
    [Header("Identity")]
    public string eventId;

    [Header("Display")]
    public string title;

    [TextArea(2, 5)]
    public string description;

    public Sprite icon;

    public string buttonText = "Great!";

    [Header("Conditions")]
    public int minimumCoins;
    public int minimumPigs;

    [Header("Rewards")]
    public int rewardCoins;

    [Header("Behaviour")]
    public bool repeatable = false;
}