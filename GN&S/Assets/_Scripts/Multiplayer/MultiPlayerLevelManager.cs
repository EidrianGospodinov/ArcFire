using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using _Scripts.Multiplayer;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class MultiPlayerLevelManager : MonoBehaviourPunCallbacks
{
    [SerializeField] public Timer timer;
    [SerializeField] public Transform[] PlayerTransformsArr = new Transform[4];
    Transform playerTransform;

    public int maxKills = 3;

    public GameObject gameOverPopup;
    public Text winnerTitleText;

    public Text winnerBodyText;

    [Header("Chat")] 
    [SerializeField] private Transform chatPanel;

    public Chat chat;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var playerList = PhotonNetwork.PlayerList;
        var localPlayer = PhotonNetwork.LocalPlayer;
        
        int localIndex = Array.IndexOf(playerList, localPlayer);
        var gameManagerInstance = GameManager.Instance;

        PhotonNetwork.Instantiate("MultiPlayerPref",
            PlayerTransformsArr[localIndex].position, Quaternion.identity);
        
        //InitPlayerSetSpawnPoint();
        
        timer.OnTimerComplete += OnGameTimerFinished;
        if (PhotonNetwork.IsMasterClient)
        {
            timer.StartTimer();
        }
        
        chatPanel.gameObject.SetActive(true);
        
        var authenticationValues = new Photon.Chat.AuthenticationValues(PhotonNetwork.LocalPlayer.NickName);
        chat.userName = PhotonNetwork.LocalPlayer.NickName;
        chat.ChatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", authenticationValues);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("joned room");
        base.OnJoinedRoom();
    }
    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
        chat.ChatClient.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("Lobby");
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        if (PhotonNetwork.PlayerList.Length <= 1)
        {
            EndGameWithNoKillCountReached();
            StorePersonalBest();
        }
        Debug.Log($"{otherPlayer} left the room");
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer.GetScore() == maxKills)
        {
            WinLoseTextUpdate(targetPlayer);
            EndGame();
            StorePersonalBest();
        }
    }
    

    private void OnDestroy()
    {
        timer.OnTimerComplete -= OnGameTimerFinished;
    }

    private void EndGameWithNoKillCountReached()
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

    private void OnGameTimerFinished() => EndGameWithNoKillCountReached();
    


    

    void StorePersonalBest()
    {
        int currentScore = PhotonNetwork.LocalPlayer.GetScore();
        PlayerData playerData = GameManager.Instance.playerData;
        if (currentScore > playerData.bestScore)
        {
            playerData.username = PhotonNetwork.LocalPlayer.NickName;
            playerData.bestScore = currentScore;
            playerData.bestScoreDate = DateTime.UtcNow.ToString();
            playerData.totalPlayerInGame = PhotonNetwork.CurrentRoom.PlayerCount;
            playerData.roomName = PhotonNetwork.CurrentRoom.Name;
            
            GameManager.Instance.SavePlayerData();
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

    
    public void NewGame()
    {
        //clean up the scene
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            PhotonNetwork.DestroyAll();
            

            //disable ui for all players
            photonView.RPC("DisableGameOverUI", RpcTarget.All);


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
