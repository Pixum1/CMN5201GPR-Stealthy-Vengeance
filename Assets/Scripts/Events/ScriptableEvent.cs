using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/ScriptableEvent")]
public class ScriptableEvent : ScriptableObject
{
    private List<ScriptableEventListener> eventListener;
    public void Subscribe(ScriptableEventListener _listener)
    {
        eventListener.Add(_listener);
    }
    public void Unsubscribe(ScriptableEventListener _listener)
    {
        eventListener.Remove(_listener);
    }
    public void RaiseEvent()
    {
        for (int i = 0; i < eventListener.Count; i++)
        {
            eventListener[i].Response();
        }
    }
}
