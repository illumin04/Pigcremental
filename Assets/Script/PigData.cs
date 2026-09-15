using UnityEngine;

[CreateAssetMenu(
    fileName = "NewPigData",
    menuName = "Pig Game/Pig Data"
)]
public class PigData : ScriptableObject
{
    public string pigName;

    public PigType pigType;

    public Sprite icon;

    // 场景里实际生成的猪
    public GameObject prefab;

    // 每秒产出
    public float productionPerSecond;

    public double basePrice;
}