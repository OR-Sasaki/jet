using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour
{
    [SerializeField] private HingeJoint hingeJointA;
    [SerializeField] private HingeJoint hingeJointB;
    [SerializeField] private Rigidbody jetRigidbody;
    [SerializeField] private GameObject jetEffect;
    [SerializeField] private GameObject[] bodies;

    // 挙動の設定
    private float jetForce = 5f;
    private float moterForce = 1000f;
    private float moterTargetVelocity = 1000f;
    public float maxJetFuel = 100f;
    public float remainJetFuel = 100f;
    private float consumeJetFuel = 100f;
    private float jetFuelRecoveryTime = 5f;
    private float remainJetFuelRecoveryTime = 0f;
    private float jetFuelRecoverySpeed = 3f;
    private float checkPointDistance = 3f;
    private float forceAreaForce = 10f;

    // 各種データ
    private int _reachedCheckPointIndex = 0;

    void ApplySettings()
    {
        maxJetFuel = GameSettings.I.MaxJetFuel;
        consumeJetFuel = GameSettings.I.ConsumeJetFuel;
        jetForce = GameSettings.I.JetForce;
        moterForce = GameSettings.I.MotorForce;
        moterTargetVelocity = GameSettings.I.MoterTargetVelocity;
        jetFuelRecoveryTime = GameSettings.I.JetFuelRecoveryTime;
        jetFuelRecoverySpeed = GameSettings.I.JetFuelRecoverySpeed;
        forceAreaForce = GameSettings.I.ForceAreaForce;
    }

    void Start()
    {
        // _reachedCheckPointIndex以下のチェックポイントをActiveにする
        GameManager.I.checkPoints
            .Where(t => t.Index <= _reachedCheckPointIndex)
            .ToList()
            .ForEach(t => t.SetActive(true));
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

        // ジェット燃料の回復
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

        // チェックポイント間の移動
        if(Input.GetKeyDown(KeyCode.Q))
        {
            MovePreviousCheckPoint();
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            MoveNextCheckPoint();
        }
    }

    void MovePreviousCheckPoint()
    {
        var checkPoints = GameManager.I.checkPoints.Reverse();
        var pos = this.transform.position;
        foreach (var t in checkPoints)
        {
            // チェックポイントの位置が現在の位置よりも奥にある場合はスキップ
            if (pos.z <= t.transform.position.z) continue;

            // チェックポイントが現在位置とかなり近い場合はスキップ
            if (Vector3.Distance(t.transform.position, this.transform.position) < checkPointDistance) continue;

            pos.y = t.transform.position.y;
            pos.z = t.transform.position.z;
            SetPosition(pos);
            break;
        }
    }

    void MoveNextCheckPoint()
    {
        var checkPoints = GameManager.I.checkPoints;
        var pos = this.transform.position;
        foreach (var t in checkPoints)
        {
            // 到達済みのチェックポイントより先にはいけない
            if (t.Index > _reachedCheckPointIndex) continue;

            // チェックポイントの位置が現在の位置より手前にある場合はスキップ
            if (t.transform.position.z <= this.transform.position.z) continue;

            // チェックポイントが現在位置とかなり近い場合はスキップ
            if (Vector3.Distance(t.transform.position, this.transform.position) < checkPointDistance) continue;

            pos.y = this.transform.position.y;
            pos.z = t.transform.position.z;
            SetPosition(pos);
            break;
        }
    }

    void SetPosition(Vector3 position)
    {
        // bodiesの全てのTransformの位置を更新
        foreach (var body in bodies)
        {
            body.transform.position = position;
            body.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }
        this.transform.position = position;
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
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

    // チェックポイントに到達したときの処理
    public void OnTriggerEnter(Collider collision)
    {
        OnTriggerCheckpoint(collision);
        OnTriggerDeadArea(collision);
    }

    private void OnTriggerCheckpoint(Collider other)
    {
        if (!other.gameObject.TryGetComponent<CheckPoint>(out var checkPoint)) return;
        if (checkPoint.Index <= _reachedCheckPointIndex) return;

        _reachedCheckPointIndex = checkPoint.Index;
        checkPoint.SetActive(true);
    }

    private void OnTriggerDeadArea(Collider other)
    {
        if (!other.gameObject.TryGetComponent<DeadArea>(out var deadArea)) return;

        // 体の位置をリセット
        MovePreviousCheckPoint();

        // ジェット燃料をリセット
        remainJetFuel = maxJetFuel;
        remainJetFuelRecoveryTime = jetFuelRecoveryTime;
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("OnTriggerStay: " + other.gameObject.name);
        
        // ForceAreaとの相互作用
        if (other.gameObject.TryGetComponent<ForceArea>(out var forceArea))
        {
            // ForceAreaの前方方向に力を加える
            Vector3 forceDirection = forceArea.transform.forward;
            
            // jetRigidbodyに力を加える
            jetRigidbody.AddForce(forceDirection * forceAreaForce, ForceMode.Force);
        }
    }
}
