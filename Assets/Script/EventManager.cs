using UnityEngine;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    public GameManager gameManager;
    public EventPopup eventPopup;

    public EventData[] events;

    private bool popupOpen = false;

    private HashSet<string> triggeredEvents =
        new HashSet<string>();

    void Update()
    {
        if (popupOpen)
            return;

        CheckEvents();
    }

    void CheckEvents()
    {
        foreach (EventData gameEvent in events)
        {
            if (gameEvent == null)
                continue;

            if (!gameEvent.repeatable &&
                triggeredEvents.Contains(gameEvent.eventId))
            {
                continue;
            }

            if (IsEventEligible(gameEvent))
            {
                TriggerEvent(gameEvent);
                break;
            }
        }
    }

    bool IsEventEligible(EventData gameEvent)
    {
        if (gameManager.coins < gameEvent.minimumCoins)
            return false;

        if (gameManager.pigs < gameEvent.minimumPigs)
            return false;

        return true;
    }

    void TriggerEvent(EventData gameEvent)
    {
        if (!gameEvent.repeatable)
        {
            triggeredEvents.Add(gameEvent.eventId);
        }

        popupOpen = true;

        eventPopup.ShowEvent(
            gameEvent,
            this
        );
    }

    public void EventFinished()
    {
        popupOpen = false;
    }
}