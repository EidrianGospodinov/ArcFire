using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        
        //convert JSON to binary array with utf-8
        byte[] jsonBytes = Encoding.UTF8.GetBytes(serialisedDataString);
        var base64String = Convert.ToBase64String(jsonBytes);
        
        
        File.WriteAllText(filePath, base64String);
    }

    public void LoadPlayerData()
    {
        if(!File.Exists(filePath))
        {
            playerData = new PlayerData();
            SavePlayerData();
            return;
        }
        string fileContents = File.ReadAllText(filePath);

        //we are converting the data the opposite to bite and then to the original value
        var jsonBytes = Convert.FromBase64String(fileContents);

        var jsonString = Encoding.UTF8.GetString(jsonBytes);
        
        playerData = JSON.ParseString(jsonString).Deserialize<PlayerData>();
        
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
