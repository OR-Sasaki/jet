using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    private Player player;

    void Awake()
    {
        player = FindObjectsByType<Player>(FindObjectsSortMode.None)[0];
    }

    void OnTriggerEnter(Collider other)
    {
        player.OnTriggerEnter(other);
    }
}
