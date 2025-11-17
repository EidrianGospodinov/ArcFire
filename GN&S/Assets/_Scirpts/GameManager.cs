using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PlayerTransformDict
{//no longer needed
    public Transform location;
    public bool isUsed;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] public Transform[] PlayerTransformsArr;
    [SerializeField] public List<PlayerTransformDict> PlayerTransforms;//no longer needed
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public void SetCursorState(bool isGameMode)
    {
        if (isGameMode)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
    public List<GameObject> PlayersInGame = new List<GameObject>();
    public bool isGamePausedOrOver = false;
}
