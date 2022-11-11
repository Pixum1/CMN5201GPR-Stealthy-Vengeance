using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScriptableEventListener : MonoBehaviour
{
    [SerializeField] private ScriptableEvent m_ScriptableEvent;
    [SerializeField] private UnityEvent m_UnityEvent;

    #region Subscribe / Unsubscribe
    private void OnEnable()
    {
        m_ScriptableEvent.Subscribe(this);
    }
    private void OnDisable()
    {
        m_ScriptableEvent.Unsubscribe(this);
    }
    #endregion

    public void Response()
    {
        m_UnityEvent.Invoke();
    }
}
