using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
public class EventPopup : MonoBehaviour
{
    public Image iconImage;
    public GameObject popupLayer;
    public GameManager gameManager;

    [Header("UI")]
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text rewardText;
    public TMP_Text buttonText;

    [Header("Animation")]
    public CanvasGroup popupCanvasGroup;
    public RectTransform popupPanel;

    public float showDuration = 0.25f;
    public float hideDuration = 0.20f;

    private EventData currentEvent;
    private EventManager eventManager;

    public void ShowEvent(
        EventData gameEvent,
        EventManager manager)
    {
        currentEvent = gameEvent;
        eventManager = manager;

        if (gameEvent.icon != null)
{
    iconImage.gameObject.SetActive(true);
    iconImage.sprite = gameEvent.icon;
}
else
{
    iconImage.gameObject.SetActive(false);
}
        titleText.text =
            gameEvent.title;

        descriptionText.text =
            gameEvent.description;

        buttonText.text =
            gameEvent.buttonText;

        if (gameEvent.rewardCoins > 0)
        {
            rewardText.gameObject.SetActive(true);

            rewardText.text =
                $"+{gameEvent.rewardCoins} Coins";
        }
        else
        {
            rewardText.gameObject.SetActive(false);
        }

        popupLayer.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(ShowPopup());
    }

    public void ClaimEvent()
    {
        if (currentEvent.rewardCoins > 0)
        {
            gameManager.AddCoins(
                currentEvent.rewardCoins
            );
        }

        StopAllCoroutines();
        StartCoroutine(HidePopup());
    }

    IEnumerator ShowPopup()
    {
        float timer = 0f;

        popupCanvasGroup.alpha = 0f;

        popupPanel.localScale =
            new Vector3(0.9f, 0.9f, 1f);

        while (timer < showDuration)
        {
            timer += Time.deltaTime;

            float t =
                timer / showDuration;

            float smoothT =
                1f - Mathf.Pow(1f - t, 3f);

            popupCanvasGroup.alpha =
                smoothT;

            float scale =
                Mathf.Lerp(
                    0.9f,
                    1f,
                    smoothT
                );

            popupPanel.localScale =
                new Vector3(
                    scale,
                    scale,
                    1f
                );

            yield return null;
        }

        popupCanvasGroup.alpha = 1f;
        popupPanel.localScale = Vector3.one;
    }

    IEnumerator HidePopup()
    {
        float timer = 0f;

        while (timer < hideDuration)
        {
            timer += Time.deltaTime;

            float t =
                timer / hideDuration;

            float smoothT =
                t * t;

            popupCanvasGroup.alpha =
                Mathf.Lerp(
                    1f,
                    0f,
                    smoothT
                );

            float scale =
                Mathf.Lerp(
                    1f,
                    0.94f,
                    smoothT
                );

            popupPanel.localScale =
                new Vector3(
                    scale,
                    scale,
                    1f
                );

            yield return null;
        }

        popupCanvasGroup.alpha = 0f;
        popupLayer.SetActive(false);

        eventManager.EventFinished();
    }
}