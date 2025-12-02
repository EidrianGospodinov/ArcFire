using NUnit.Framework;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using _Scripts.Multiplayer;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MultiplayerLobby : MonoBehaviourPunCallbacks
{

    //todo fix this nonsense -- activate panel
    [SerializeField] Transform LoginPanel;
    [SerializeField] Transform SelectionPanel;
    [SerializeField] Transform CreateRoomPanel;
    [SerializeField] Transform InsideRoomPanel;
    [SerializeField] Transform ListRoomsPanel;
    [SerializeField] Transform ChatPanel;

    [SerializeField] InputField roomNameInput;
    [SerializeField] InputField playerNameInput;

    [SerializeField] GameObject textPrefab;
    [SerializeField] GameObject startGameButton;
    [SerializeField] Transform insideRoomPlayerList;
    [SerializeField] Transform listRoomPanel;
    [SerializeField] GameObject roomEntryPrefab;
    [SerializeField] Transform ListRoomPanelContent;

    private string playerName;
    private Dictionary<string, RoomInfo> cachedRoomList;
    private bool canPlaySolo = false;


    public Chat chat;


    private void Start()
    {
        ActivatePanel("Login");
        playerNameInput.text = playerName = string.Format($"Player {Random.Range(1, 10000000)}");

        cachedRoomList = new Dictionary<string, RoomInfo>();
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public void StartGameClicked()
    {
        if (PhotonNetwork.PlayerList.Length >= 2 || canPlaySolo)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel("Multiplayer");
        }
        else
        {
            Debug.Log("Not Enough players");
        }

        {
            
        }
    }
    public void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        if (roomList == null)
        {
            return;
        }
        foreach (RoomInfo room in roomList)
        {
            if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                cachedRoomList.Remove(room.Name);
            }
            else
            {
                cachedRoomList[room.Name] = room;
            }
        }


    }

    public MultiplayerLobby()
    {
    }
    public void LoginButtonClicked()
    {
        PhotonNetwork.LocalPlayer.NickName = playerName = playerNameInput.text;

        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        print("Successfull connection to the master server!");
        ActivatePanel("Selection");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        print("Disconnected from master server");
        ActivatePanel("Login");
    }
    public void ActivatePanel(string panelName)
    {
        DisableAllPanels();

        if (panelName == LoginPanel.gameObject.name)
            LoginPanel.gameObject.SetActive(true);
        else if (panelName == SelectionPanel.gameObject.name)
            SelectionPanel.gameObject.SetActive(true);
        else if (panelName == CreateRoomPanel.gameObject.name)
            CreateRoomPanel.gameObject.SetActive(true);
        else if (panelName == InsideRoomPanel.gameObject.name)
            InsideRoomPanel.gameObject.SetActive(true);
        else if (panelName == ListRoomsPanel.gameObject.name)
            ListRoomsPanel.gameObject.SetActive(true);
        else if (panelName == ChatPanel.gameObject.name)
            ChatPanel.gameObject.SetActive(true);
    }

    private void DisableAllPanels()
    {
        LoginPanel.gameObject.SetActive(false);
        SelectionPanel.gameObject.SetActive(false);
        CreateRoomPanel.gameObject.SetActive(false);
        InsideRoomPanel.gameObject.SetActive(false);
        ListRoomsPanel.gameObject.SetActive(false);
        ChatPanel.gameObject.SetActive(false);
    }
        
    
    public void DisconnectButtonClicked()
    {
        PhotonNetwork.Disconnect();
    }
    public void GoToCreateRoom()
    {
        ActivatePanel("CreateRoom");
    }
    public void CreateARoom()
    {
        RoomOptions roomOptions= new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.IsVisible = true;

        PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions);
    }
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        print("Room has been created!");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        print("failed to create room");
    }
    //List<GameObject> PlayerList = new List<GameObject>();
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        var authenticationValues = new Photon.Chat.AuthenticationValues(PhotonNetwork.LocalPlayer.NickName);
        chat.userName = PhotonNetwork.LocalPlayer.NickName;
        chat.ChatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", authenticationValues);
        
        print("joined room!");
        ActivatePanel("InsideRoom");
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);

        foreach (var player in PhotonNetwork.PlayerList)
        {
            var playerListEntry = Instantiate(textPrefab, insideRoomPlayerList);
            //PlayerList.Add(playerListEntry);
            playerListEntry.GetComponent<Text>().text = player.NickName;
            playerListEntry.name = player.NickName;
        }
    }
    public void ListRoomClicked()
    {
        print("list room button clicked");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        print("joined lobby!");
        ActivatePanel("ListRooms");
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        Debug.Log($"Room count: {roomList.Count}");

        DestroyChildren(ListRoomPanelContent);

        UpdateCachedRoomList(roomList);


        foreach (var room in cachedRoomList)
        {
            var newRoomEntry = Instantiate(roomEntryPrefab, ListRoomPanelContent);
            var newRoomEntryScript = newRoomEntry.GetComponent<RoomEntry>();
            newRoomEntryScript.roomName = room.Key;
            newRoomEntryScript.roomText.text = string.Format("[{0} - ({1} / {2})]", room.Key, room.Value.PlayerCount, room.Value.MaxPlayers);
        }


    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        chat.ChatClient.Disconnect();
        print("left from this room!");
        

        DestroyChildren(insideRoomPlayerList);

        ActivatePanel("CreateRoom");
    }
    public void LeaveRoomClicked()
    {
        PhotonNetwork.LeaveRoom();
    }
    public void LeaveLobbyClicked()
    {
        PhotonNetwork.LeaveLobby();

    }
    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
        print("Left Lobby");
        DestroyChildren(ListRoomPanelContent);
        DestroyChildren(insideRoomPlayerList);
        cachedRoomList.Clear();
        ActivatePanel("Selection");
    }
    public void DestroyChildren(Transform parent)
    {
        foreach(Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
    public void OnJoinRandomRoomClicked()
    {
        var randRoomSuccess = PhotonNetwork.JoinRandomRoom();
        if (randRoomSuccess)
        {// can be done with override OnJoinRandomFailed
            print("fail to join random room!");
        }
        else
        {
            print("successfully joinded a random room!");
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        print("Player joined room!");
        var playerListEntry = Instantiate(textPrefab, insideRoomPlayerList);
        //PlayerList.Add(playerListEntry);
        playerListEntry.GetComponent<Text>().text = newPlayer.NickName;
        playerListEntry.name = newPlayer.NickName;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        print("Player left room");
        foreach (Transform child in insideRoomPlayerList)
        {
            //terrible for bigger games
            if(child.name == otherPlayer.NickName)
            {
                Destroy(child.gameObject);
                break;
            }
        }

    }
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void CanPlaySoloClicked(bool isToggleOn)
    {
        canPlaySolo = isToggleOn;
    }
    
}
