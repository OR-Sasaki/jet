using System.Collections;
using UnityEngine;
using VContainer;
using ILogger = Root.Service.ILogger;

namespace Title.Manager
{
    public class ManGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private float minInterval = 0.5f;
        [SerializeField] private float maxInterval = 2f;
        [SerializeField] private float minScale = 0.8f;
        [SerializeField] private float maxScale = 2.6f;
        [SerializeField] private Vector3 spawnPoint0 = new(-10f, -5f, 50f);
        [SerializeField] private Vector3 spawnPoint1 = new(10f, -5f, 50f);
        [SerializeField] private Vector3 spawnPoint2 = new(10f, 5f, 50f);
        [SerializeField] private Vector3 spawnPoint3 = new(-10f, 5f, 50f);
        [SerializeField] private Vector3 rotation = Vector3.zero;

        private ILogger _logger;
        [SerializeField] private bool _isGenerating;

        [Inject]
        public void Construct(ILogger logger)
        {
            _logger = logger;
        }

        private void Start()
        {
            if (prefab == null)
            {
                _logger?.LogError("ManGenerator: Prefabが設定されていません");
                return;
            }

            _isGenerating = true;
            StartCoroutine(GenerateRoutine());
        }

        private void OnDestroy()
        {
            _isGenerating = false;
        }

        private IEnumerator GenerateRoutine()
        {
            while (_isGenerating)
            {
                float interval = Random.Range(minInterval, maxInterval);
                yield return new WaitForSeconds(interval);

                GenerateMan();
            }
        }

        private void GenerateMan()
        {
            float u = Random.Range(0f, 1f);
            float v = Random.Range(0f, 1f);

            // 双線形補間で四角形内の位置を計算
            Vector3 position = (1 - u) * (1 - v) * spawnPoint0
                             + u * (1 - v) * spawnPoint1
                             + u * v * spawnPoint2
                             + (1 - u) * v * spawnPoint3;

            Quaternion quaternion = Quaternion.Euler(rotation);
            GameObject man = Instantiate(prefab, position, quaternion);

            float scale = Random.Range(minScale, maxScale);
            man.transform.localScale = Vector3.one * scale;

            man.SetActive(true);

            _logger?.Log($"ManGenerator: Man生成 (Position: {position}, Scale: {scale}, Rotation: {rotation})");
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;

            // 四角形を描画
            Gizmos.DrawLine(spawnPoint0, spawnPoint1);
            Gizmos.DrawLine(spawnPoint1, spawnPoint2);
            Gizmos.DrawLine(spawnPoint2, spawnPoint3);
            Gizmos.DrawLine(spawnPoint3, spawnPoint0);
        }
#endif
    }
}

