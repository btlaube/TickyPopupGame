using System;
using TMPro;
using UnityEngine;

/// <summary>
/// The taskbar clock. By default it only moves when you call Penalty()/AddMinutes()
/// (so reading the email isn't a race). Set secondsPerMinute > 0 to let it tick.
/// </summary>
public class GameClock : MonoBehaviour
{
    public static GameClock I { get; private set; }

    [SerializeField] TMP_Text label;
    [SerializeField] int startHour24 = 16;
    [SerializeField] int startMinute = 30;
    [SerializeField] int deadlineHour24 = 17;
    [SerializeField] int penaltyMinutes = 5;
    [Tooltip("Real seconds per in-game minute. 0 = clock only moves from Penalty()/AddMinutes().")]
    [SerializeField] float secondsPerMinute = 0f;

    public event Action DeadlinePassed;
    public bool OutOfTime => totalMinutes >= deadlineHour24 * 60;

    float totalMinutes;
    float tick;
    bool deadlineFired;

    void Awake()
    {
        I = this;
        totalMinutes = startHour24 * 60 + startMinute;
        Refresh();
    }

    void Update()
    {
        if (secondsPerMinute <= 0f) return;
        tick += Time.deltaTime;
        while (tick >= secondsPerMinute && !deadlineFired)
        {
            tick -= secondsPerMinute;
            AddMinutes(1);
        }
    }

    public void Penalty() => AddMinutes(penaltyMinutes);

    public void AddMinutes(int minutes)
    {
        totalMinutes += minutes;
        Refresh();
        if (!deadlineFired && OutOfTime)
        {
            deadlineFired = true;
            DeadlinePassed?.Invoke();
        }
    }

    void Refresh()
    {
        if (label == null) return;
        int total = Mathf.FloorToInt(totalMinutes);
        int h24 = (total / 60) % 24;
        int m = total % 60;
        int h12 = h24 % 12;
        if (h12 == 0) h12 = 12;
        label.text = $"{h12}:{m:00} {(h24 >= 12 ? "PM" : "AM")}";
    }
}
