using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private float jetForce = 5f;
    [SerializeField] private HingeJoint hingeJointA;
    [SerializeField] private HingeJoint hingeJointB;
    [SerializeField] private float moterForce = 1000f;
    [SerializeField] private float moterTargetVelocity = 1000f;
    [SerializeField] private Rigidbody jetRigidbody;

    void Update()
    {
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

        if (Input.GetKey(KeyCode.Space))
        {
            jetRigidbody.AddForce(jetRigidbody.transform.up * jetForce, ForceMode.Force);
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
