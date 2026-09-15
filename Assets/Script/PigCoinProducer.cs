using UnityEngine;

public class PigCoinProducer : MonoBehaviour
{
    // 每次产出时间的随机波动范围
    // 例如 1 coin/s 时，每枚金币大约间隔 0.8 ~ 1.2 秒
    public float minTimeMultiplier = 0.8f;
    public float maxTimeMultiplier = 1.2f;

    private float timer;

    private GameObject coinPrefab;
    private RectTransform coinTarget;
    private GameManager gameManager;

    // 这只猪真正的每秒产出
    private float productionPerSecond = 1f;


    public void Initialize(
        GameManager manager,
        GameObject prefab,
        RectTransform target,
        float productionRate
    )
    {
        gameManager = manager;
        coinPrefab = prefab;
        coinTarget = target;

        productionPerSecond = productionRate;

        ResetTimer();
    }


    // 给 GameManager 的总产出 UI 使用
    public float GetCoinRate()
    {
        return productionPerSecond;
    }


    void Update()
    {
        if (coinPrefab == null)
            return;

        if (productionPerSecond <= 0f)
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
        if (productionPerSecond <= 0f)
        {
            timer = Mathf.Infinity;
            return;
        }

        // 例如：
        // production = 1/s
        // 平均每 1 秒产生一次
        //
        // production = 5/s
        // 平均每 0.2 秒产生一次

        float baseInterval =
            1f / productionPerSecond;

        float multiplier =
            Random.Range(
                minTimeMultiplier,
                maxTimeMultiplier
            );

        timer =
            baseInterval * multiplier;
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

        // 金币首先出现在猪的位置
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