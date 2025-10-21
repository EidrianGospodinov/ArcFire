using NUnit.Framework;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerLobby : MonoBehaviourPunCallbacks
{

    //todo fix this noncence -- activate panel
    [SerializeField] Transform LoginPanel;
    [SerializeField] Transform SelectionPanel;
    [SerializeField] Transform CreateRoomPanel;
    [SerializeField] Transform InsideRoomPanel;
    [SerializeField] Transform ListRoomsPanel;

    [SerializeField] InputField roomNameInput;
    [SerializeField] InputField playerNameInput;

    [SerializeField] GameObject textPrefab;
    [SerializeField] Transform insideRoomPlayerList;
    [SerializeField] Transform listRoomPanel;
    [SerializeField] GameObject roomEntryPrefab;
    [SerializeField] Transform ListRoomPanelContent;

    private string playerName;

    private void Start()
    {
        ActivatePanel("Login");
        playerNameInput.text = playerName = string.Format($"Player {Random.Range(1, 10000000)}");
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
        LoginPanel.gameObject.SetActive(false);
        SelectionPanel.gameObject.SetActive(false);
        CreateRoomPanel.gameObject.SetActive(false);
        InsideRoomPanel.gameObject.SetActive(false);
        ListRoomsPanel.gameObject.SetActive(false);

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
    List<GameObject> PlayerList = new List<GameObject>();
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        print("joined room!");
        ActivatePanel("InsideRoom");
        foreach (var player in PhotonNetwork.PlayerList)
        {
            var playerListEntry = Instantiate(textPrefab, insideRoomPlayerList);
            PlayerList.Add(playerListEntry);
            playerListEntry.GetComponent<Text>().text = player.NickName;

        }
    }
    public void ListRoomClicked()
    {
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
        print("Room count: {}" + roomList.Count);
        foreach (var room in roomList)
        {
            var newRoomEntry = Instantiate(roomEntryPrefab, ListRoomPanelContent);
            var newRoomEntryScript = newRoomEntry.GetComponent<RoomEntry>();
            newRoomEntryScript.roomName = room.Name;
            newRoomEntryScript.roomText.text = string.Format("[{0} - ({1} / {2})]", room.Name, room.PlayerCount, room.MaxPlayers);
        }


    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        print("left from this room, joined different one");
        ActivatePanel("CreateRoom");

        var playerTextGameObject = PlayerList.Find(x => x.GetComponent<Text>().text == this.playerName);
        
        PlayerList.Remove(playerTextGameObject);
        Destroy(playerTextGameObject);

    }
    public void LeaveRoom()
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
        ActivatePanel("Selection");
    }
}
