using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using _Scripts.Multiplayer;
using Leguar.TotalJSON;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

[System.Serializable]
public struct PlayerTransformDict
{//no longer needed
    public Transform location;
    public bool isUsed;
}

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }
    public PlayerData playerData;
    public string filePath;
    public GlobalLeaderboard globalLeaderboard;
    public WeeklyLeaderboard weeklyLeaderboard;
    
    
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
        gameObject.AddComponent<PhotonView>();
        var isIdAllocated = PhotonNetwork.AllocateViewID(photonView);
        if (!isIdAllocated)
        {
            Debug.Log($"Failed to allocated new id, current ID: {photonView.ViewID}");
        }
        LoadPlayerData();
        LoginToPlayerData();
    }

    private void OnDestroy()
    {
        if (PhotonNetwork.IsConnected && photonView != null)
        {
            PhotonNetwork.Destroy(photonView);
        }
    }

    private void LoginToPlayerData()
    {
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest()
        {
            CreateAccount = true,
            CustomId = playerData.uid,
        };
        Debug.LogError($"player data unique id {playerData.uid}");
        
        PlayFabClientAPI.LoginWithCustomID(request, PlayFabLoginResult, PlayFabLoginError);
    }

    void PlayFabLoginResult(LoginResult result)
    {
        Debug.Log($"Login successfully completed: {result.ToJson()}");
    }

    void PlayFabLoginError(PlayFabError error)
    {
        Debug.Log($"Login failed: {error.ErrorMessage}");
    }

    public void SavePlayerData()
    {
        string serialisedDataString = JSON.Serialize(playerData).CreateString();
        
        //convert JSON to binary array with utf-8
        byte[] jsonBytes = Encoding.UTF8.GetBytes(serialisedDataString);
        var base64String = Convert.ToBase64String(jsonBytes);
        
        //Encrypt the base 64 string with AES
        var inputBytes = Encoding.UTF8.GetBytes(base64String);
        var encryptedBytes = DataSecurity.Encrypt(inputBytes);
        
        //convert the encrypted bytes to base64 string
        string finalEncryptedString = Convert.ToBase64String(encryptedBytes);
        
        
        File.WriteAllText(filePath, finalEncryptedString);
    }

    public void LoadPlayerData()
    {
        Debug.LogError(filePath);
        if(!File.Exists(filePath))
        {
            playerData = new PlayerData();
            SavePlayerData();
            return;
        }
        string fileContents = File.ReadAllText(filePath);

        //we are converting the data the opposite to bite and then to the original value
        var encryptedBytes = Convert.FromBase64String(fileContents);

        var decryptedBytes = DataSecurity.Decrypt(encryptedBytes);


        string base64String = Encoding.UTF8.GetString(decryptedBytes);

        var jsonBytes = Convert.FromBase64String(base64String);
        string jsonString = Encoding.UTF8.GetString(jsonBytes);
        
        playerData = JSON.ParseString(jsonString).Deserialize<PlayerData>();
        
    }
    public void SetCursorState(bool isGameMode)
    {
        if (isGameMode)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            isGamePausedOrOver = false;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            isGamePausedOrOver = true;
        }
    }
    public bool isGamePausedOrOver = false;
    
    
    // Used to delete the data from the inspector
    [ContextMenu("DELETE Saved Player Data")]
    public void DeletePlayerData()
    {
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
                Debug.Log($"Player data successfully deleted from: {filePath}");
            }
            catch (IOException e)
            {
                Debug.LogError($"Error deleting data file (IOException): {e.Message}");
            }
            catch (UnauthorizedAccessException e)
            {
                Debug.LogError($"Error deleting data file (Permission Denied): {e.Message}");
            }
            catch (Exception e)
            {
                Debug.LogError($"An unknown error occurred while deleting the data: {e.Message}");
            }
        }
        else
        {
            Debug.Log($"No player data file found to delete at: {filePath}");
        }
    }
    public void RequestKick(Photon.Realtime.Player target)
    {
        photonView.RPC("RequestKickRPC", RpcTarget.MasterClient, target.ActorNumber);
    }

    [PunRPC]
    void RequestKickRPC(int actorNumber, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        var target = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
        if (target == null)
            return;

        PhotonNetwork.CloseConnection(target);
    }
}
