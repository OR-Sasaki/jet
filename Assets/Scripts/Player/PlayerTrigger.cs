using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    private Player player;

    void Awake()
    {
        player = this.gameObject.GetComponentInParent<Player>();
    }
}
