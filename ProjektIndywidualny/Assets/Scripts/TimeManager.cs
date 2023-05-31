using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    private float startTime;
    private float endTime;
    private float bestTime = Mathf.Infinity;

    private bool isTiming;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartTimer()
    {
        //Rozpoczęcie odliczania od początku gry
        startTime = Time.time;
        isTiming = true;
    }

    public void StopTimer()
    {
        if (isTiming)
        {
            endTime = Time.time;
            float currentTime = endTime - startTime;
            if (currentTime < bestTime)
            {
                bestTime = currentTime;
                PlayerPrefs.SetFloat("BestTime", bestTime);
            }
            isTiming = false;
        }
    }

    public float GetBestTime()
    {
        bestTime = PlayerPrefs.GetFloat("BestTime", Mathf.Infinity);
        return bestTime;
    }
}
