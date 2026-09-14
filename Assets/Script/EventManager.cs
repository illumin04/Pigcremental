using System;
using System.Collections.Generic;
using UnityEngine;

// A snapshot of what ALREADY happened; UI never executes its effects.
public sealed class EventNotice
{
    public string title, description, reward, buttonText;
    public Sprite icon;
    public bool overridePanelColor;
    public Color panelColor;
    public GameObject extraVisualPrefab;
    public EventNotice(EventData data, string result)
    {
        title = data.title; description = data.description; reward = result;
        buttonText = data.buttonText; icon = data.icon;
        overridePanelColor = data.overridePanelColor; panelColor = data.panelColor;
        extraVisualPrefab = data.extraVisualPrefab;
    }
}

public class EventManager : MonoBehaviour
{
    public GameManager gameManager;
    public EventPopup eventPopup;
    public EventData[] events;
    [Min(.05f)] public float checkInterval = .25f;

    private readonly HashSet<string> triggeredEvents = new HashSet<string>();
    private readonly HashSet<string> failedEvents = new HashSet<string>();
    private readonly Dictionary<string, double> nextTimes = new Dictionary<string, double>();
    private readonly Queue<EventNotice> notices = new Queue<EventNotice>();
    private float timer;
    private bool popupOpen;

    private void Start()
    {
        if (!gameManager || !eventPopup)
        { StopWithError("Assign GameManager and EventPopup."); return; }
        var ids = new HashSet<string>();
        if (events == null) events = new EventData[0];
        foreach (var data in events)
        {
            if (!data) continue; // Existing empty Inspector slots are harmless.
            if (string.IsNullOrWhiteSpace(data.eventId) || !ids.Add(data.eventId))
            { StopWithError("Missing/duplicate Event Id: " + data.name + ". Give every event a unique ID."); return; }
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = Mathf.Max(.05f, checkInterval);
            CheckEvents(); // Always runs, even while a notice is open.
        }
        ShowNextNotice();
    }

    private void CheckEvents()
    {
        foreach (var data in events)
        {
            if (!data || failedEvents.Contains(data.eventId)) continue;
            if (!data.repeatable && triggeredEvents.Contains(data.eventId)) continue;
            if (nextTimes.TryGetValue(data.eventId, out var next) && Time.timeAsDouble < next) continue;
            try
            {
                if (gameManager.coins < data.minimumCoins || gameManager.pigs < data.minimumPigs) continue;
                bool eligible = true;
                if (data.extraConditions != null)
                    foreach (var condition in data.extraConditions)
                    {
                        if (!condition) throw new InvalidOperationException("Empty Extra Conditions slot in " + data.name);
                        if (!condition.IsMet(gameManager)) { eligible = false; break; }
                    }
                if (!eligible) continue;
                // Validate references before applying any of this event's effects.
                if (data.extraEffects != null)
                    foreach (var effect in data.extraEffects)
                        if (!effect) throw new InvalidOperationException("Empty Extra Effects slot in " + data.name);

                triggeredEvents.Add(data.eventId);
                nextTimes[data.eventId] = Time.timeAsDouble + Math.Max(1f, data.cooldownSeconds);
                var results = new List<string>();
                if (data.rewardCoins != 0)
                {
                    if ((long)gameManager.coins + data.rewardCoins > int.MaxValue ||
                        (long)gameManager.coins + data.rewardCoins < int.MinValue)
                        throw new OverflowException("Coin reward overflow.");
                    gameManager.AddCoins(data.rewardCoins);
                    results.Add(data.rewardCoins.ToString("+0;-0;0") + " Coins");
                }
                if (data.extraEffects != null)
                    foreach (var effect in data.extraEffects)
                    {
                        string result = effect.Execute(gameManager);
                        if (!string.IsNullOrWhiteSpace(result)) results.Add(result);
                    }
                notices.Enqueue(new EventNotice(data, string.Join("\n", results)));
            }
            catch (Exception error)
            {
                // Arbitrary code cannot be rolled back universally. Never retry a
                // partially applied event automatically; other events keep running.
                failedEvents.Add(data.eventId);
                Debug.LogError("Event failed: " + data.name + ". Disabled for this session; some effects may already have applied. " + error, data);
            }
        }
    }

    private void ShowNextNotice()
    {
        if (popupOpen || notices.Count == 0) return;
        popupOpen = true;
        eventPopup.ShowEvent(notices.Dequeue(), this);
    }

    public void EventFinished() { popupOpen = false; }

    private void StopWithError(string message)
    { Debug.LogError(message, this); enabled = false; }
}
