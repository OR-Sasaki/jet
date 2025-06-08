using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] Rigidbody _rb;

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _rb.MovePosition(transform.position + transform.forward * _speed * Time.deltaTime);
        }
    }
}
