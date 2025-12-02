using System;
using System.Collections.Generic;
using System.IO;
using Leguar.TotalJSON;
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
    public PlayerData playerData;
    public string filePath;
    
    
   // [SerializeField] public Transform[] PlayerTransformsArr = new Transform[4];//load them from the level manager in the future
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

    private void Start()
    {
        LoadPlayerData();
    }

    public void SavePlayerData()
    {
        string serialisedDataString = JSON.Serialize(playerData).CreateString();
        File.WriteAllText(filePath, serialisedDataString);
    }

    public void LoadPlayerData()
    {
        if(!File.Exists(filePath))
        {
            playerData = new PlayerData();
            SavePlayerData();
        }
        string fileContents = File.ReadAllText(filePath);
        playerData = JSON.ParseString(fileContents).Deserialize<PlayerData>();
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
