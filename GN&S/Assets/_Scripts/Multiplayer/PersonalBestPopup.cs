using System;
using UnityEngine;
using UnityEngine.UI;

public class PersonalBestPopup : MonoBehaviour
{
    [SerializeField] private GameObject scoreHolder;
    [SerializeField] private GameObject noScoreHolder;
    [SerializeField] private Text userName;
    [SerializeField] private Text bestScore;
    [SerializeField] private Text date;
    [SerializeField] private Text totalPlayers;
    [SerializeField] private Text roomName;

    private void OnEnable()
    {
        UpdatePersonalBesUI();
    }

    public void UpdatePersonalBesUI()
    {
        PlayerData playerData = GameManager.Instance.playerData;
        if (playerData != null)
        {
            userName.text = playerData.username;
            bestScore.text = playerData.bestScore.ToString();
            date.text = playerData.bestScoreDate;
            totalPlayers.text = playerData.totalPlayerInGame.ToString();
            roomName.text = playerData.roomName;
            
            scoreHolder.SetActive(true);
            noScoreHolder.SetActive(false);
        }
        else
        {
            scoreHolder.SetActive(false);
            noScoreHolder.SetActive(true);
        }

    }
}
