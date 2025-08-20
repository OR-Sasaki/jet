using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private GameObject flag;

    public int Index { get; set; } = 0;

    public void SetActive(bool isActive)
    {
        flag.SetActive(isActive);
    }
}
