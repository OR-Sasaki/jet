using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/GameSettings")]
public class GameSettings : ScriptableObject
{
    private static GameSettings instance;
    public static GameSettings I =>
    instance ??= Resources.Load<GameSettings>("GameSettings");

    void Awake()
    {
        instance = this;
    }

    [Header("ジェットの力")]
    public float JetForce = 50f;

    [Header("ボムの力")]
    public float BombForce = 10f;

    [Header("プレイヤーの曲がる力")]
    public float MotorForce = 1000f;
    public float MoterTargetVelocity = 1000f;

    [Header("ジェット燃料の最大値")]
    public float MaxJetFuel = 100f;

    [Header("ジェット燃料の秒間消費量")]
    public float ConsumeJetFuel = 100f;

    [Header("移動を止めてからジェット燃料が回復するまでの秒数")]
    public float JetFuelRecoveryTime = 5f;

    [Header("ジェット燃料の回復速度")]
    public float JetFuelRecoverySpeed = 3f;

    [Header("ForceAreaの力の強さ")]
    public float ForceAreaForce = 10f;
}
