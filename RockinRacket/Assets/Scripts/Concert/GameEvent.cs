using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvent : MonoBehaviour 
{
    


    public static event EventHandler<GameEventArgs> OnEventStart; //Starting of an event
    public static event EventHandler<GameEventArgs> OnEventFail; //Fail of an event
    public static event EventHandler<GameEventArgs> OnEventMiss; //Miss of an event, due to game state change
    public static event EventHandler<GameEventArgs> OnEventCancel; //Cancelation of an event, such as items used
    public static event EventHandler<GameEventArgs> OnEventComplete; //Successful events
    public static event EventHandler<GameEventArgs> OnEventOpen;
    public static event EventHandler<GameEventArgs> OnEventClose;

    public static void EventStart(GameEvent eventData)
    {
        OnEventStart?.Invoke(null, new GameEventArgs(eventData));
    }

    public static void EventFail(GameEvent eventData)
    {
        OnEventFail?.Invoke(null, new GameEventArgs(eventData));
    }

    public static void EventCancel(GameEvent eventData)
    {
        OnEventCancel?.Invoke(null, new GameEventArgs(eventData));
    }

    public static void EventMiss(GameEvent eventData)
    {
        OnEventMiss?.Invoke(null, new GameEventArgs(eventData));
    }

    public static void EventComplete(GameEvent eventData)
    {
        OnEventComplete?.Invoke(null, new GameEventArgs(eventData));
    }

    public static void EventOpened(GameEvent eventData)
    {
        OnEventOpen?.Invoke(null, new GameEventArgs(eventData));
    }

    public static void EventClosed(GameEvent eventData)
    {
        OnEventClose?.Invoke(null, new GameEventArgs(eventData));
    }

    public float songActivationTime = 0;
    public int activationNumber = 0;  

    public float duration; //Time till the event automatically ends
    public float remainingDuration; 
    public bool infiniteDuration;
    public bool isActiveEvent = false;
    //public GameObject GameEventPrefab;
    public EventType eventType = EventType.Song;
    private Coroutine durationCoroutine = null;


    public virtual void Activate()
    {
        isActiveEvent = true;
        remainingDuration = duration;
        if (!infiniteDuration) {
            remainingDuration = duration;
            durationCoroutine = StartCoroutine(EventDurationCountdown());
        }
        EventStart(this);
        
    }

    public virtual void End()
    {
        isActiveEvent = false;
        if (durationCoroutine != null) {
            StopCoroutine(durationCoroutine);
        }
        EventFail(this);
        EventClosed(this);
    }

    public virtual void Miss()
    {
        isActiveEvent = false;
        if (durationCoroutine != null) {
            StopCoroutine(durationCoroutine);
        }
        EventMiss(this);
        EventClosed(this);
    }

    public virtual void Complete()
    {
        isActiveEvent = false;
        if (durationCoroutine != null) {
            StopCoroutine(durationCoroutine);
        }
        EventComplete(this);
        EventClosed(this);
    }

    public virtual void OpenEvent()
    {
        EventOpened(this);
    }

    public virtual void CloseEvent()
    {
        EventClosed(this);
    }
    
    //Since this is based on Time.deltaTime; it will actually already be affected by
    // the pausing of the time scale = 0, but maybe useful to keep the code around
    //
    private IEnumerator EventDurationCountdown()
    {
        while (remainingDuration > 0)
        {
            yield return null;
            remainingDuration -= Time.deltaTime;

            if (remainingDuration <= 0)
            {
               End();
            }
        }
    }

    /*
    public virtual void Pause()
    {
        if (isActiveEvent && durationCoroutine != null) {
            StopCoroutine(durationCoroutine);
            durationCoroutine = null;
        }
    }

    public virtual void Resume()
    {
        if (isActiveEvent && !infiniteDuration && remainingDuration > 0) {
            durationCoroutine = StartCoroutine(EventDurationCountdown());
        }
    }
    */

    void OnEnable()
    {
        GameEventManager.Instance.OnSecondPassed += CheckActivationTime;
        //TimeEvents.OnGamePaused += Pause;
        //TimeEvents.OnGameResumed += Resume;
    }

    void OnDisable()
    {
        GameEventManager.Instance.OnSecondPassed -= CheckActivationTime;
        //TimeEvents.OnGamePaused -= Pause;
        //TimeEvents.OnGameResumed -= Resume;
    }

    void CheckActivationTime(float currentTime)
    {
        if (!isActiveEvent && currentTime >= songActivationTime && activationNumber == GameEventManager.Instance.songNumber)
        {
            GameEventManager.Instance.OnSecondPassed -= CheckActivationTime;
            Activate();
        }
}
}

[System.Serializable]
public enum EventType
{
    Song, //
    Intermission, //
    All, //
}

public class GameEventArgs : EventArgs
{
    public GameEvent eventObject { get; private set; }
    public float Duration { get; set; }

    public GameEventArgs(GameEvent eventData)
    {
        eventObject = eventData;
    }

    public GameEventArgs(GameEvent eventData, float duration)
    {
        Duration = duration;
    }
}
