using UnityEngine;

namespace Title.View
{
    public class FlyingObject : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float destroyZ = -50f;

        private void Update()
        {
            transform.position += transform.up * (speed * Time.deltaTime);

            if (transform.position.z < destroyZ)
            {
                Destroy(gameObject);
            }
        }
    }
}

