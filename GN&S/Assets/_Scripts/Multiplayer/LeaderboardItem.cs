using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Multiplayer
{
    public class LeaderboardItem : MonoBehaviour
    {
        [SerializeField] private Text order;
        [SerializeField] private Text username;
        [SerializeField] private Text score;

        public void SetScore(int order, string username, int score)
        {
            this.order.text = order.ToString();
            this.username.text = username;
            this.score.text = score.ToString();          
            
        }
    }
}