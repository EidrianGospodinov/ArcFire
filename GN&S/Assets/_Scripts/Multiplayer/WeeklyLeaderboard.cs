using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace _Scripts.Multiplayer
{
    public class WeeklyLeaderboard : MonoBehaviour
    {
        private int maxResults = 5;
        [SerializeField] WeeklyLeaderboardPopup leaderboardPopup;
        public void GetLeaderboard()
        {
            GetLeaderboardRequest request = new GetLeaderboardRequest()
            {
                MaxResultsCount = maxResults,
                StatisticName = "Weakly Leaderboard - Most Kills",
            };
            PlayFabClientAPI.GetLeaderboard(request, PlayFabGetLeaderboardResult, PlayFabGetLeaderboardError);
        }
        private void PlayFabGetLeaderboardError(PlayFabError obj)
        {
            Debug.LogError($"PlayFab - error occurred while getting leaderboard: {obj.Error}");
        }

        private void PlayFabGetLeaderboardResult(GetLeaderboardResult getLeaderboardResult)
        {
            Debug.Log("PlayFab - Get Weekly Leaderboard completed");
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
                        StatisticName = "Weakly Leaderboard - Most Kills",
                        Value = score,
                    }
                }
            };
            PlayFabClientAPI.UpdatePlayerStatistics(request, PlayFabUpdateStatsResult, error => PlayFabUpdateStatsError(error, score));
        }

        private void PlayFabUpdateStatsError(PlayFabError obj, int score)
        {
            Debug.LogError($"PlayFab - error occurred while submitting weekly score: {obj.Error}");
            SubmitScore(score);
        }

        void PlayFabUpdateStatsResult(UpdatePlayerStatisticsResult result)
        {
            Debug.Log("PlayFab - weekly Score submitted");
        }
    }
}