using UnityEngine;

public class Coinpickup : MonoBehaviour
{
    public float dropDuration = 0.35f;
    public float flyDuration = 0.55f;
    public float dropDistance = 60f;

    private RectTransform rectTransform;

    private GameManager gameManager;
    private RectTransform coinTarget;

    private Vector2 dropStart;
    private Vector2 dropTarget;

    private float timer;

    private enum CoinState
    {
        Dropping,
        Flying
    }

    private CoinState state;

    public void Initialize(
        GameManager manager,
        RectTransform target
    )
    {
        gameManager = manager;
        coinTarget = target;

        rectTransform = GetComponent<RectTransform>();

        dropStart = rectTransform.anchoredPosition;

        // 金币随机掉到猪附近。
        Vector2 randomDirection =
            Random.insideUnitCircle.normalized;

        dropTarget =
            dropStart + randomDirection * dropDistance;

        timer = 0f;
        state = CoinState.Dropping;
    }

    void Update()
    {
        if (state == CoinState.Dropping)
        {
            UpdateDrop();
        }
        else
        {
            UpdateFly();
        }
    }

    void UpdateDrop()
    {
        timer += Time.deltaTime;

        float t = timer / dropDuration;

        if (t >= 1f)
        {
            rectTransform.anchoredPosition = dropTarget;

            timer = 0f;
            state = CoinState.Flying;

            return;
        }

        // 从猪的位置移动到附近。
        Vector2 position =
            Vector2.Lerp(dropStart, dropTarget, t);

        // 加一个小抛物线，让它像被弹出来。
        float bounce =
            Mathf.Sin(t * Mathf.PI) * 30f;

        rectTransform.anchoredPosition =
            position + Vector2.up * bounce;
    }

    void UpdateFly()
    {
        timer += Time.deltaTime;

        float t = timer / flyDuration;

        // SmoothStep 会比普通 Lerp 更有“吸过去”的感觉。
        float smoothT =
            Mathf.SmoothStep(0f, 1f, t);

        transform.position =
            Vector3.Lerp(
                transform.position,
                coinTarget.position,
                smoothT
            );

        if (t >= 1f)
        {
            gameManager.AddCoins(1);

            Destroy(gameObject);
        }
    }
}