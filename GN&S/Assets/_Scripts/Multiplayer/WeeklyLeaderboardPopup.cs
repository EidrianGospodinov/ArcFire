using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;

namespace _Scripts.Multiplayer
{
    public class WeeklyLeaderboardPopup : MonoBehaviour
    {
        [SerializeField] private GameObject scoreHolder;
        [SerializeField] private GameObject noScoreText;
        [SerializeField] private GameObject leaderboardItem;

        private void OnEnable()
        {
            GameManager.Instance.weeklyLeaderboard.GetLeaderboard();
        }
        public void UpdateUI(List<PlayerLeaderboardEntry> entries)
        {
            if (entries.Count > 0)
            {
                DestroyChildren(scoreHolder.transform);
                for (int i = 0; i < entries.Count; i++)
                {
                    GameObject newLeaderboardItem = Instantiate(leaderboardItem, Vector3.zero, Quaternion.identity, scoreHolder.transform);
                    newLeaderboardItem.GetComponent<LeaderboardItem>().SetScore(i+1, entries[i].DisplayName, entries[i].StatValue);
                }
                scoreHolder.SetActive(true);
                noScoreText.SetActive(false);
            }
            else
            {
                scoreHolder.SetActive(false);
                noScoreText.SetActive(true);
            }
        }
        public void DestroyChildren(Transform parent)
        {
            foreach(Transform child in parent)
            {
                Destroy(child.gameObject);
            }
        }
    }
}