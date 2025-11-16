using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class MultiPlayerLevelManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform playerTransform;
    [SerializeField] private Timer timer;

    public int maxKills = 3;

    public GameObject gameOverPopup;
    public Text winnerTitleText;

    public Text winnerBodyText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.PlayersInGame.Add(
            PhotonNetwork.Instantiate("MultiPlayerPref", playerTransform.position, Quaternion.identity));

        timer.OnTimerComplete += OnGameTimerFinished;
        timer.StartTimer();

    }

    private void OnDestroy()
    {
        timer.OnTimerComplete -= OnGameTimerFinished;
    }

    private void OnGameTimerFinished()
    {
        if (PhotonNetwork.InRoom)
        {
            var playerList = PhotonNetwork.PlayerList;
            var highestScorers = new List<Photon.Realtime.Player>();
            int highestScore = 0;
            foreach (var player in playerList)
            {
                int playerScore = player.GetScore();
                if (playerScore > highestScore)
                {
                    highestScorers.Clear();
                    highestScorers.Add(player);
                    highestScore = playerScore;
                }
                else if (playerScore == highestScore)
                {
                    highestScorers.Add(player);
                }
            }

            if (highestScorers.Count == 1)
            {
                WinLoseTextUpdate(highestScorers[0]);
            }
            else
            {
                winnerTitleText.text = "Draw";
                if (highestScore == 0)
                {
                    winnerBodyText.text = "No Winners!";
                }
                else
                {
                    StringBuilder nicknameList = new StringBuilder();
                    for (int i = 0; i < highestScorers.Count; i++)
                    {
                        nicknameList.Append(highestScorers[i].NickName);
        
                        if (i < highestScorers.Count - 2)
                        {
                            nicknameList.Append(", "); // Comma separation for all but the last two
                        }
                        else if (i == highestScorers.Count - 2)
                        {
                            nicknameList.Append(" and "); // "and" before the last player
                        }
                    }
                    winnerBodyText.text = $"The game is a tie! Players {nicknameList}" +
                                          $" finished with an equal score of {highestScore}.";
                }
                
                
            }
            EndGame();   
        }
    }


    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer.GetScore() == maxKills)
        {
            WinLoseTextUpdate(targetPlayer);
            EndGame();
        }
    }

    private void WinLoseTextUpdate(Photon.Realtime.Player targetPlayer)
    {
        if (PhotonNetwork.NickName == targetPlayer.NickName)
        {
            winnerTitleText.text = "You";
        }
        else
        {
            winnerTitleText.text = targetPlayer.NickName;
        }
    }

    private void EndGame()
    {
        var gameManagerInstance =  GameManager.Instance;
        gameManagerInstance.SetCursorState(false);
        gameManagerInstance.isGamePausedOrOver = true;
            
        gameOverPopup.SetActive(true);
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
        GameManager.Instance.PlayersInGame.ForEach(PhotonNetwork.Destroy);
        GameManager.Instance.PlayersInGame.Clear();
        
        //disable ui for all players
        photonView.RPC("DisableGameOverUI", RpcTarget.All);


        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel("TransitionScene");
            //PhotonNetwork.LoadLevel("Multiplayer");
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
