using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int pigs = 1;
    public int coins = 1;

    public TMP_Text pigValueText;
    public TMP_Text coinValueText;

    public GameObject pigPrefab;
    public RectTransform pigArea;

     public GameObject coinPrefab;
    public RectTransform coinTarget;

    public void AddCoins(int amount)
{
    coins += amount;
    UpdateUI();
}

    public void FeedPig()
    {
        pigs += 1;

        SpawnPig();

        UpdateUI();
    }

    void SpawnPig()
    {
        GameObject newPig = Instantiate(pigPrefab, pigArea);
        PigCoinProducer producer =
    newPig.GetComponent<PigCoinProducer>();

producer.Initialize(
    this,
    coinPrefab,
    coinTarget
);
        RectTransform pigRect = newPig.GetComponent<RectTransform>();

        float margin = 50f;

        float x = Random.Range(
            -pigArea.rect.width / 2f + margin,
            pigArea.rect.width / 2f - margin
        );

        float y = Random.Range(
            -pigArea.rect.height / 2f + margin,
            pigArea.rect.height / 2f - margin
        );

        pigRect.anchoredPosition = new Vector2(x, y);
    }

     void UpdateUI()
    {
        pigValueText.text = pigs.ToString();
        coinValueText.text = coins.ToString();
    }
}
