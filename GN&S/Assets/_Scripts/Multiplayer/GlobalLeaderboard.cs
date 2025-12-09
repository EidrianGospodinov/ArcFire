using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace _Scripts.Multiplayer
{
    public class GlobalLeaderboard : MonoBehaviour
    {
        private int maxResults = 5;
        [SerializeField] LeaderboardPopup leaderboardPopup;
        public void GetLeaderboard()
        {
            GetLeaderboardRequest request = new GetLeaderboardRequest()
            {
                MaxResultsCount = maxResults,
                StatisticName = "Global Leaderboard - Most Kills",
            };
            PlayFabClientAPI.GetLeaderboard(request, PlayFabGetLeaderboardResult, PlayFabGetLeaderboardError);
        }

        private void PlayFabGetLeaderboardError(PlayFabError obj)
        {
            Debug.LogError($"PlayFab - error occurred while getting leaderboard: {obj.Error}");
        }

        private void PlayFabGetLeaderboardResult(GetLeaderboardResult getLeaderboardResult)
        {
            Debug.Log("PlayFab - Get Leaderboard completed");
            leaderboardPopup.UpdateUI(getLeaderboardResult.Leaderboard);
        }

        public void SubmitScore(int score)
        {
            UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest()
            {
                Statistics = new List<StatisticUpdate>
                {

                    new StatisticUpdate()
                    {
                        StatisticName = "Global Leaderboard - Most Kills",
                        Value = score,
                    }
                }
            };
            PlayFabClientAPI.UpdatePlayerStatistics(request, PlayFabUpdateStatsResult, PlayFabUpdateStatsError);
        }

        private void PlayFabUpdateStatsError(PlayFabError obj)
        {
            Debug.LogError($"PlayFab - error occurred while submitting score: {obj.Error}");
        }

        void PlayFabUpdateStatsResult(UpdatePlayerStatisticsResult result)
        {
            Debug.Log("PlayFab - Score submitted");
        }
    }

}