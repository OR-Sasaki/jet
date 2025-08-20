using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager I
    {
        get
        {
            if (_instance != null) return _instance;

            _instance = FindAnyObjectByType<GameManager>();
            if (_instance != null) return _instance;

            throw new Exception("GameManager not found in the scene. Please add a GameManager component to a GameObject.");
        }
    }

    // チェックポイントの一覧
    public CheckPoint[] checkPoints = Array.Empty<CheckPoint>();

    void Awake()
    {
        for(var i = 0; i < checkPoints.Length; i++)
        {
            checkPoints[i].SetActive(false);
            checkPoints[i].Index = i;
        }
    }
}
