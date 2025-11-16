using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class MultiPlayerLevelManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform playerTransform;

    public int maxKills = 3;

    public GameObject gameOverPopup;
    public Text winnerText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PhotonNetwork.Instantiate("MultiPlayerPref", playerTransform.position, Quaternion.identity);
    }
    
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer.GetScore() == maxKills)
        {
            winnerText.text = targetPlayer.NickName;
            
            GameManager.Instance.SetCursorState(false);
            
            gameOverPopup.SetActive(true);
        }
    }

    public void LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("Lobby");
    }
    public void NewGame()
    {
        //clean up the scene
        PhotonNetwork.DestroyAll();
        
        //disable ui for all players
        photonView.RPC("DisableGameOverUI", RpcTarget.All);


        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel("Multiplayer");
        }
        else
        {
            Debug.LogWarning("Not enough players to restart.");
        }
    }
    [PunRPC]
    private void DisableGameOverUI()
    {
        if (gameOverPopup != null)
        {
            gameOverPopup.SetActive(false);
        }
    }
}
