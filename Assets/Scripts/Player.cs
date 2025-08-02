using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private HingeJoint hingeJointA;
    [SerializeField] private HingeJoint hingeJointB;
    [SerializeField] private Rigidbody jetRigidbody;
    [SerializeField] private GameObject jetEffect;

    private float jetForce = 5f;
    private float moterForce = 1000f;
    private float moterTargetVelocity = 1000f;

    public float maxJetFuel = 100f;
    public float remainJetFuel = 100f;
    private float consumeJetFuel = 100f;
    private float jetFuelRecoveryTime = 5f;
    private float remainJetFuelRecoveryTime = 0f;
    private float jetFuelRecoverySpeed = 3f;

    void ApplySettings()
    {
        maxJetFuel = GameSettings.I.MaxJetFuel;
        consumeJetFuel = GameSettings.I.ConsumeJetFuel;
        jetForce = GameSettings.I.JetForce;
        moterForce = GameSettings.I.MotorForce;
        moterTargetVelocity = GameSettings.I.MoterTargetVelocity;
        jetFuelRecoveryTime = GameSettings.I.JetFuelRecoveryTime;
        jetFuelRecoverySpeed = GameSettings.I.JetFuelRecoverySpeed;
    }

    void Update()
    {
        ApplySettings();

        // 体の曲げ
        if (Input.GetKey(KeyCode.A))
        {
            SetHingeMoter(true);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            SetHingeMoter(false);
        }
        else
        {
            hingeJointA.useMotor = false;
            hingeJointB.useMotor = false;
        }

        // ジェット
        if (Input.GetKey(KeyCode.Space) && remainJetFuel > 0)
        {
            remainJetFuel = Mathf.Max(remainJetFuel - consumeJetFuel * Time.deltaTime, 0);
            jetEffect.SetActive(true);
            jetRigidbody.AddForce(jetRigidbody.transform.up * jetForce, ForceMode.Force);
        }
        else
        {
            jetEffect.SetActive(false);
        }

        if (Input.anyKey)
        {
            remainJetFuelRecoveryTime = jetFuelRecoveryTime;
        }
        else
        {
            remainJetFuelRecoveryTime -= Time.deltaTime;

            if (remainJetFuelRecoveryTime <= 0)
                remainJetFuel = Mathf.Min(remainJetFuel + jetFuelRecoverySpeed * Time.deltaTime, maxJetFuel);
        }
    }

    void SetHingeMoter(bool direction)
    {
        hingeJointA.useMotor = true;
        var motor = hingeJointA.motor;
        motor.force = moterForce;
        motor.targetVelocity = (direction ? 1 : -1) * moterTargetVelocity;
        hingeJointA.motor = motor;

        hingeJointB.useMotor = true;
        var motorB = hingeJointB.motor;
        motorB.force = moterForce;
        motorB.targetVelocity = (direction ? -1 : 1) * moterTargetVelocity;
        hingeJointB.motor = motorB;
    }
}
