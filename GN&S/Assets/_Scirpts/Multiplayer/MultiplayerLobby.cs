using Photon.Pun;
using UnityEngine;

public class MultiplayerLobby : MonoBehaviourPunCallbacks
{
    public MultiplayerLobby()
    {
    }
    public void LoginButtonClicked()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        print("Successfull connection to the master server!");
    }
}
