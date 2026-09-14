using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int pigs = 1;

    public TMP_Text pigCountText;

    public void FeedPig()
    {
        pigs += 1;
        UpdateUI();
    }

    void UpdateUI()
    {
        pigCountText.text = $"Pigs: {pigs}";
    }
}
