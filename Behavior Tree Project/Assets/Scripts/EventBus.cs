using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventBus : Singleton<EventBus>
{
    private Dictionary<string, UnityEvent> m_EventDictionary;
    private static int nextEventID = 17; // start at an arbitrary non-zero int

    public override void Awake()
    {
        base.Awake();
        Instance.Init();
    }

    private void Init()
    {
        if (Instance.m_EventDictionary == null)
        {
            Instance.m_EventDictionary = new Dictionary<string, UnityEvent>();
        }
    }

    public static int GetEventID()
    {
        return nextEventID++;
    }

    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (Instance.m_EventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            Instance.m_EventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (Instance.m_EventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if (Instance.m_EventDictionary.TryGetValue(eventName, out thisEvent))
        {
            Debug.Log("Triggering " + eventName);
            thisEvent.Invoke();
        }
    }

    public static void ScheduleTrigger(string eventName, float secondsFromNow)
    {
        EventBus.Instance.StartCoroutine(EventBus.Instance.DelayTrigger(eventName, secondsFromNow));
    }

    IEnumerator DelayTrigger(string eventName, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        TriggerEvent(eventName);
    }
}