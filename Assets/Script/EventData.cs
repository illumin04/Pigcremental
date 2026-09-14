using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEvent", menuName = "Pigcremental/Event")]
public class EventData : ScriptableObject
{
    [Header("Identity - unique for each event")]
    public string eventId;
    [Header("Display")]
    public string title;
    [TextArea(2, 5)] public string description;
    public Sprite icon;
    public string buttonText = "Great!";
    [Header("Conditions - ALL must pass")]
    public int minimumCoins;
    public int minimumPigs;
    public EventCondition[] extraConditions = new EventCondition[0];
    [Header("Effects - applied immediately, NOT on confirmation")]
    public int rewardCoins;
    public EventEffect[] extraEffects = new EventEffect[0];
    [Header("Behaviour")]
    public bool repeatable = false;
    [Min(1f)] public float cooldownSeconds = 60f;
    [Header("Optional appearance")]
    public bool overridePanelColor;
    public Color panelColor = new Color(1f, .89f, .66f, 1f);
    public GameObject extraVisualPrefab;

    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(eventId)) eventId = Guid.NewGuid().ToString("N");
        cooldownSeconds = Mathf.Max(1f, cooldownSeconds);
    }
}
