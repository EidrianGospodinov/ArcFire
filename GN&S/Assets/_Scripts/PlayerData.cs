using UnityEngine;

public class PlayerData
{
    public string uid;
    public string username;
    public int bestScore;
    public string bestScoreDate;
    public int totalPlayerInGame;
    public string roomName;

    public PlayerData()
    {
        uid = System.Guid.NewGuid().ToString();
    }
}
