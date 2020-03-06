using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventBus : Singleton<EventBus>
{
    private Dictionary<string, UnityEvent> m_EventDictionary;
    private static int nextEventID = 1; // non-zero seems best

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
            //Debug.Log("Triggering " + eventName);
            thisEvent.Invoke();
        }
    }

    public static void ScheduleTrigger(string eventName, float secondsFromNow)
    {
        // because this is a static function, we can't do the following:
        //StartCoroutine(DelayTrigger(eventName, secondsFromNow));
        // we need an instance of an object to run coroutines on, like so:
        EventBus.Instance.StartCoroutine(EventBus.Instance.DelayTrigger(eventName, secondsFromNow));

        // NOTE: this is a case where a singleton is a better solution than a simple static class
        // we wouldn't be able to run coroutines here if our event bus was just a static class
    }

    IEnumerator DelayTrigger(string eventName, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        TriggerEvent(eventName);
    }
}