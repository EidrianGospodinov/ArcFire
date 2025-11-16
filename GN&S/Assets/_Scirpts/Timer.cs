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
                // Decrease time by the amount passed since the last frame
                timeRemaining -= Time.deltaTime; 
                
                DisplayTime(timeRemaining);
            }
            else
            {
                // Timer reached zero
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
                
                TimerComplete();
            }
        }
    }

    // Converts seconds into a display format (e.g., 01:30)
    void DisplayTime(float timeToDisplay)
    {
        // Math.Floor is used to handle floating point issues and ensure the number is positive
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        // Formats the output with leading zeros (e.g., 01:05)
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
