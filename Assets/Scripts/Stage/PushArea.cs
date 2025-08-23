using UnityEngine;

public class PushArea : MonoBehaviour
{
    [Header("Push Settings")]
    [SerializeField] private float velocity = 15f; // PushAreaの速度

    [Header("Gizmo Settings")]
    [SerializeField] private float arrowLength = 5f; // 矢印の長さ
    [SerializeField] private float arrowWidth = 0.5f; // 矢印の幅
    [SerializeField] private Color arrowColor = Color.blue; // 矢印の色

    // 速度を取得するプロパティ
    public float Velocity => velocity;

    private void OnDrawGizmos()
    {
        // Gizmoの色を設定
        Gizmos.color = arrowColor;

        Vector3 start = transform.position - transform.forward * 0.5f;
        Vector3 end = start + transform.forward * arrowLength;
        Vector3 right = transform.up * arrowWidth;

        // 矢印の頂点を定義
        Vector3[] arrowPoints = new Vector3[4];
        arrowPoints[0] = end - right; // 矢印の左端
        arrowPoints[1] = end + right; // 矢印の右端
        arrowPoints[2] = end; // 矢印の先端
        arrowPoints[3] = start; // 矢印の基点

        // 矢印の本体（線）を描画
        Gizmos.DrawLine(arrowPoints[3], arrowPoints[2]); // 基点から先端まで

        // 矢印の頭部分を描画
        Gizmos.DrawLine(arrowPoints[0], arrowPoints[2]); // 左端から先端まで
        Gizmos.DrawLine(arrowPoints[1], arrowPoints[2]); // 右端から先端まで

        // 矢印の頭部分を閉じる線を描画（オプション）
        Gizmos.DrawLine(arrowPoints[0], arrowPoints[1]); // 左端から右端まで
    }
}
