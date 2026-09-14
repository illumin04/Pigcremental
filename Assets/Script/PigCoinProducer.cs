using UnityEngine;

public class PigCoinProducer : MonoBehaviour
{
    public float minProduceTime = 2f;
    public float maxProduceTime = 5f;

    private float timer;

    private GameObject coinPrefab;
    private RectTransform coinTarget;
    private GameManager gameManager;

    public void Initialize(
        GameManager manager,
        GameObject prefab,
        RectTransform target
    )
    {
        gameManager = manager;
        coinPrefab = prefab;
        coinTarget = target;

        ResetTimer();
    }
    // calculate the coin rate.
    public float GetCoinRate()
{
    float averageProduceTime =
        (minProduceTime + maxProduceTime) / 2f;

    return 1f / averageProduceTime;
}
    void Update()
    {
        if (coinPrefab == null)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            ProduceCoin();
            ResetTimer();
        }
    }

    void ResetTimer()
    {
        timer = Random.Range(
            minProduceTime,
            maxProduceTime
        );
    }

    void ProduceCoin()
    {
        RectTransform pigRect =
            GetComponent<RectTransform>();

        GameObject coin =
            Instantiate(
                coinPrefab,
                transform.parent
            );

        RectTransform coinRect =
            coin.GetComponent<RectTransform>();

        // 金币首先出现在猪当前位置。
        coinRect.anchoredPosition =
            pigRect.anchoredPosition;

        Coinpickup pickup =
            coin.GetComponent<Coinpickup>();

        pickup.Initialize(
            gameManager,
            coinTarget
        );
    }
}