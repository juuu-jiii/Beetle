using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// Names of events that can be listened for.
public enum Events
{
    MarbleMatch,
    ProjectileMatch,
    TargetMatch,
    MarbleSpawn,
    GameOver,
}

/// <summary>
/// Singleton to streamline event handling throughout a level.
/// </summary>
public class EventManager : MonoBehaviour
{
    // Key/value pairs with an enum for keys - used as an alternative to
    // error-prone strings.
    /// <summary>
    /// Key: name of the event; Value: UnityEvent i.e. the event object itself.
    /// </summary>
    private Dictionary<Events, UnityEvent> eventDict;
    private static EventManager instance;

    // Singleton implementation
    /// <summary>
    /// Singleton instance of EventManager.
    /// </summary>
    public static EventManager Instance
    {
        get
        {
            if (!instance)
            {
                // FindObjectOfType returns the first active loaded object of a
                // specified type. It is used here to get the first EventManager
                // instance in the scene.
                instance = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!instance) Debug.LogError("Scene must have one active " +
                    "EventManager script on a GameObject.");
                else instance.Init();
            }

            return instance;
        }
    }

    /// <summary>
    /// Initialises eventDictionary if it is null.
    /// </summary>
    private void Init()
    {
        if (eventDict == null) 
            eventDict = new Dictionary<Events, UnityEvent>();
    }

    /// <summary>
    /// Assigns the specified listener to eventName, if eventName exists.
    /// Otherwise, a new key/value pair is created accordingly.
    /// </summary>
    /// <param name="eventName">
    /// Name of the event to assign a listener to. If this does not exist in
    /// eventDict, a new entry is created.
    /// </param>
    /// <param name="callback">
    /// The listener to assign to eventName.
    /// </param>
    public static void StartListening(Events eventName, UnityAction callback)
    {
        UnityEvent thisEvent = null;
        
        // If ContainsKey() were used, thisEvent would need to be assigned
        // beneath the conditional check. TryGetValue() uses the out parameter
        // to accomplish this in a single line, like TryParse() does.
        if (Instance.eventDict.TryGetValue(eventName, out thisEvent)) 
            thisEvent.AddListener(callback);
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(callback);
            Instance.eventDict.Add(eventName, thisEvent);
        }
    }

    /// <summary>
    /// Removes the specified listener from eventName.
    /// </summary>
    /// <param name="eventName">
    /// Name of the event to remove the listener from.
    /// </param>
    /// <param name="callback">
    /// The listener to remove from eventName.
    /// </param>
    public static void StopListening(Events eventName, UnityAction callback)
    {
        if (!Instance) return;

        UnityEvent thisEvent = null;

        if (Instance.eventDict.TryGetValue(eventName, out thisEvent))
            thisEvent.RemoveListener(callback);
    }

    /// <summary>
    /// Fires or invokes the listeners associated with eventName.
    /// </summary>
    /// <param name="eventName">
    /// The name of the event whose listeners are to be fired or invoked.
    /// </param>
    /// <param name="marble1">
    /// First marble to be removed via GameManager.HandleCollision().
    /// </param>
    /// <param name="marble2">
    /// Second marble to be removed via GameManager.HandleCollision().
    /// </param>
    public static void TriggerEvent(Events eventName)
    {
        UnityEvent thisEvent = null;

        if (Instance.eventDict.TryGetValue(eventName, out thisEvent))
            thisEvent.Invoke();
    }
}