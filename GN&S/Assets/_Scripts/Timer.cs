using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Timer : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private float timerTotalDuration = 120f;
    private Text text;

    private float timeRemainingOLD;
    private double targetEndTime = 0.0;
    private double timerPausedAt = 0.0;

    private bool timerIsRunning = false;
    
    public event Action OnTimerComplete;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeRemainingOLD = timerTotalDuration;
        text = GetComponent<Text>();
        if (PhotonNetwork.IsMasterClient)
        {
            timerPausedAt = timerTotalDuration;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (timerIsRunning)
        {
            double timeRemaining = targetEndTime - PhotonNetwork.Time;
            DisplayTime((float)timeRemaining);
            if (timeRemaining <= 0.0)
            {
                Debug.Log("time has run out!");
                timerIsRunning = false;
                DisplayTime(0f);
                TimerComplete();
            }
        }
        else
        {
            DisplayTime((float)timerPausedAt);
        }
    }
    void DisplayTime(float timeToDisplay)
    {
        // Math.Floor keeps the number positive
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        text.text = $"{minutes:00}:{seconds:00}";
    }

    void TimerComplete()
    {
        OnTimerComplete?.Invoke();
        Debug.Log("Timer is complete! End game function activated.");
    }

    public void StartTimer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            AuthoritativeStartTimer();
            return;
        }
        photonView.RPC("RPC_StartTimerRequest", RpcTarget.MasterClient);
        
    }
    public void StopTimer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            AuthoritativeStopTimer();
            return;
        }
        photonView.RPC("RPC_StopTimerRequest", RpcTarget.MasterClient);
    }
    
    

    private void AuthoritativeStartTimer()
    {
        if(!PhotonNetwork.IsMasterClient || timerIsRunning) return;

        double durationToResume = (timerPausedAt > 0) ? timerPausedAt : timerTotalDuration;
        targetEndTime = PhotonNetwork.Time + durationToResume;

        timerPausedAt = 0.0;
        timerIsRunning = true;
        Debug.Log($"MC started timer. End time {targetEndTime}");
    }
    private void AuthoritativeStopTimer()
    {
        if (!PhotonNetwork.IsMasterClient || !timerIsRunning) return;

        timerPausedAt = targetEndTime - PhotonNetwork.Time;
        timerIsRunning = false;
        targetEndTime = 0.0; 
        
        timerPausedAt = Math.Max(0.0, timerPausedAt);
        Debug.Log($"MC stopped timer. Remaining: {timerPausedAt}");
    }
    public void RequestTimeChange(float newTime)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SetTimeRemaining(newTime);
        }
        else
        {
            photonView.RPC("RPC_SetTimeRemainingRequest", RpcTarget.MasterClient, newTime);
        }
    }
    [PunRPC]
    private void RPC_StartTimerRequest()
    {
        AuthoritativeStartTimer();
    }

    [PunRPC]
    private void RPC_StopTimerRequest()
    {
        AuthoritativeStartTimer();
    }
    [PunRPC]
    private void RPC_SetTimeRemainingRequest(float newTime)
    {
        // Executes the command requested by any client.
        SetTimeRemaining(newTime);
    }


    private void SetTimeRemaining(float time)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // Calculates a new, synchronized end time
        targetEndTime = PhotonNetwork.Time + time;
        timerIsRunning = true;
        timerPausedAt = 0.0; 
        
        Debug.Log($"MC set time remaining to {time} seconds.");
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // MASTER CLIENT: Send the current authoritative state.
            stream.SendNext(targetEndTime);
            stream.SendNext(timerIsRunning);
            stream.SendNext(timerPausedAt);
        }
        else
        {
            // NON-MASTER CLIENTS: Receive and store the authoritative state.
            targetEndTime = (double)stream.ReceiveNext();
            timerIsRunning = (bool)stream.ReceiveNext();
            timerPausedAt = (double)stream.ReceiveNext();
        }
    }
}
