using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class EventPopup : MonoBehaviour
{
    // Keep original field names and script .meta to preserve Main.unity references.
    public Image iconImage;
    public GameObject popupLayer;
    public GameManager gameManager; // Kept for old scene serialization; not used to grant rewards.
    [Header("UI")]
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text rewardText;
    public TMP_Text buttonText;
    [Header("Animation")]
    public CanvasGroup popupCanvasGroup;
    public RectTransform popupPanel;
    public float showDuration = .25f;
    public float hideDuration = .20f;
    [Header("Optional - assign a child under the panel for extra art")]
    public RectTransform extraVisualRoot;

    private EventManager eventManager;
    private Image panelImage;
    private Color originalColor;
    private GameObject extraVisual;
    private bool visible, closing;

    private void Awake()
    {
        if (popupPanel)
        {
            panelImage = popupPanel.GetComponent<Image>();
            if (panelImage) originalColor = panelImage.color;
        }
        if (popupLayer) popupLayer.SetActive(false);
    }

    public void ShowEvent(EventNotice notice, EventManager manager)
    {
        eventManager = manager; visible = true; closing = false;
        iconImage.gameObject.SetActive(notice.icon != null);
        iconImage.sprite = notice.icon;
        titleText.text = notice.title;
        descriptionText.text = notice.description;
        buttonText.text = string.IsNullOrWhiteSpace(notice.buttonText) ? "Great!" : notice.buttonText;
        rewardText.text = notice.reward;
        rewardText.gameObject.SetActive(!string.IsNullOrWhiteSpace(notice.reward));
        if (panelImage) panelImage.color = notice.overridePanelColor ? notice.panelColor : originalColor;
        ClearExtraVisual();
        if (notice.extraVisualPrefab)
        {
            if (extraVisualRoot) extraVisual = Instantiate(notice.extraVisualPrefab, extraVisualRoot, false);
            else Debug.LogWarning("Extra visual skipped: assign Extra Visual Root on EventPopup.", this);
        }
        popupLayer.SetActive(true);
        popupCanvasGroup.interactable = true;
        popupCanvasGroup.blocksRaycasts = true;
        StopAllCoroutines();
        StartCoroutine(ShowPopup());
    }

    // Keep this name so your existing claim_button OnClick binding still works.
    public void ClaimEvent()
    {
        if (!visible || closing) return;
        closing = true;
        popupCanvasGroup.interactable = false;
        // Notification only. Gameplay effects already happened in EventManager.
        StopAllCoroutines();
        StartCoroutine(HidePopup());
    }

    private IEnumerator ShowPopup()
    {
        float timer = 0f;
        popupCanvasGroup.alpha = 0f;
        popupPanel.localScale = new Vector3(.9f, .9f, 1f);
        while (timer < showDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / showDuration);
            float smoothT = 1f - Mathf.Pow(1f - t, 3f);
            popupCanvasGroup.alpha = smoothT;
            float scale = Mathf.Lerp(.9f, 1f, smoothT);
            popupPanel.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }
        popupCanvasGroup.alpha = 1f;
        popupPanel.localScale = Vector3.one;
    }

    private IEnumerator HidePopup()
    {
        float timer = 0f;
        float startAlpha = popupCanvasGroup.alpha;
        Vector3 startScale = popupPanel.localScale;
        while (timer < hideDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / hideDuration);
            float smoothT = t * t;
            popupCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, smoothT);
            popupPanel.localScale = Vector3.Lerp(startScale, new Vector3(.94f, .94f, 1f), smoothT);
            yield return null;
        }
        popupCanvasGroup.alpha = 0f;
        ClearExtraVisual();
        visible = false;
        popupLayer.SetActive(false);
        eventManager.EventFinished();
    }

    private void ClearExtraVisual()
    {
        if (!extraVisual) return;
        extraVisual.SetActive(false);
        Destroy(extraVisual);
        extraVisual = null;
    }
}
