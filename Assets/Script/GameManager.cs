using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // =========================
    // Player state
    // =========================

    // 先保留，避免你现有 UI / 其他脚本立刻坏掉
    public int pigs = 1;

    public int coins = 1;

    // 新的猪分类数据
    public PigState[] pigStates;


    // =========================
    // UI
    // =========================

    public TMP_Text pigValueText;
    public TMP_Text coinValueText;
    public TMP_Text coinRateText;


    // =========================
    // Pig spawning
    // =========================

    public GameObject pigPrefab;
    public RectTransform pigArea;


    // =========================
    // Coin visuals
    // =========================

    public GameObject coinPrefab;
    public RectTransform coinTarget;


    // =========================
    // Coins
    // =========================

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateUI();
    }


    // =========================
    // Existing normal pig button
    // =========================

    public void FeedPig()
    {
        AddPig(PigType.Normal, 1);
    }


    // =========================
    // Pig state helpers
    // =========================

    public PigState GetPigState(PigType type)
    {
        foreach (PigState pig in pigStates)
        {
            if (pig.data != null &&
                pig.data.pigType == type)
            {
                return pig;
            }
        }

        return null;
    }


    public int GetPigCount(PigType type)
    {
        PigState pig = GetPigState(type);

        if (pig == null)
            return 0;

        return pig.count;
    }

    public void TestAddFarmerPig()
{
    AddPig(PigType.Farmer, 1);
}
    public void AddPig(PigType type, int amount)
    {
        PigState pig = GetPigState(type);

        if (pig == null)
        {
            Debug.LogError(
                $"Pig type {type} is not configured in GameManager."
            );

            return;
        }

        pig.count += amount;

        // 目前每加一只猪，都生成一个视觉对象
        for (int i = 0; i < amount; i++)
        {
            SpawnPig(type);
        }

        SyncLegacyValues();

        UpdateUI();
    }


    // =========================
    // Temporary compatibility
    // =========================

    void SyncLegacyValues()
    {
        // 你现在原本 pigs 代表普通猪数量，
        // 暂时继续保持这个含义
        pigs = GetPigCount(PigType.Normal);
    }


    // =========================
    // Spawn pig
    // =========================

    void SpawnPig(PigType type)
{
    PigState pigState = GetPigState(type);

    if (pigState == null || pigState.data == null)
    {
        Debug.LogError($"PigData missing for {type}");
        return;
    }

    GameObject prefabToSpawn = pigState.data.prefab;

    if (prefabToSpawn == null)
    {
        Debug.LogError($"Prefab missing for {type}");
        return;
    }

    GameObject newPig = Instantiate(
        prefabToSpawn,
        pigArea
    );

    PigCoinProducer producer =
        newPig.GetComponent<PigCoinProducer>();

    if (producer == null)
    {
        Debug.LogError(
            $"{prefabToSpawn.name} has no PigCoinProducer component."
        );

        Destroy(newPig);
        return;
    }

    producer.Initialize(
        this,
        coinPrefab,
        coinTarget,
        pigState.data.productionPerSecond
    );

    RectTransform pigRect =
        newPig.GetComponent<RectTransform>();

    float margin = 50f;

    float x = Random.Range(
        -pigArea.rect.width / 2f + margin,
        pigArea.rect.width / 2f - margin
    );

    float y = Random.Range(
        -pigArea.rect.height / 2f + margin,
        pigArea.rect.height / 2f - margin
    );

    pigRect.anchoredPosition =
        new Vector2(x, y);
}


    // =========================
    // Coin production
    // =========================

    float GetTotalCoinRate()
    {
        PigCoinProducer[] producers =
            pigArea.GetComponentsInChildren<PigCoinProducer>();

        float totalRate = 0f;

        foreach (PigCoinProducer producer in producers)
        {
            totalRate += producer.GetCoinRate();
        }

        return totalRate;
    }


    // =========================
    // UI
    // =========================

    void UpdateUI()
    {
        // 这里暂时继续显示普通猪数量
        pigValueText.text =
            GetPigCount(PigType.Normal).ToString();

        coinValueText.text =
            coins.ToString();


        float totalRate =
            GetTotalCoinRate();

        coinRateText.text =
            $"≈ +{totalRate:0.00}/s";
    }
}