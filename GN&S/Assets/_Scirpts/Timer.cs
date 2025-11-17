using System;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private float timerTotalTime = 120f;
    private Text text;

    private float timeRemaining;

    private bool timerIsRunning = false;
    
    public event Action OnTimerComplete;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeRemaining = timerTotalTime;
        text = GetComponent<Text>();
    }
    // Update is called once per frame
    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime; 
                DisplayTime(timeRemaining);
            }
            else
            {
                // Timer reached zero
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
                DisplayTime(timeRemaining);
                TimerComplete();
            }
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
        timerIsRunning = true;
    }
    public void StopTimer()
    {
        timerIsRunning = false;
    }
}
