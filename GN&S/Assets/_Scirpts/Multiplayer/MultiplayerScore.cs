using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerScore : MonoBehaviourPunCallbacks
{

    public GameObject playerScorePrefab;

    public Transform panel;
    
    private Dictionary<int, GameObject> playerScore = new Dictionary<int, GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            player.SetScore(0);
            
            var playerScoreObj = Instantiate(playerScorePrefab, panel);
            var playerScoreObjText = playerScoreObj.GetComponent<Text>();
            playerScoreObjText.text = string.Format("{0} Score: {1}", player.NickName, player.GetScore());
            playerScore[player.ActorNumber] = playerScoreObj;
        }
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        var playerScoreObj = playerScore[targetPlayer.ActorNumber];
        var playerScoreObjText = playerScoreObj.GetComponent<Text>();
        playerScoreObjText.text = string.Format("{0} Score: {1}", targetPlayer.NickName, targetPlayer.GetScore());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
