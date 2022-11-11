using System;
using UnityEngine;

public abstract class ScriptableVariable<T> : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] protected T m_DefaultValue;
    protected T runtimeValue;
    protected Action onValueChanged;

    public virtual T Value
    {
        get
        {
            return runtimeValue;
        }
        set
        {
            runtimeValue = value;
            onValueChanged?.Invoke();
        }
    }

    public virtual void OnAfterDeserialize()
    {
        runtimeValue = m_DefaultValue;
    }

    public virtual void OnBeforeSerialize()
    {

    }

    public void Register(Action _event)
    {
        onValueChanged += _event;
    }
    public void UnRegister(Action _event)
    {
        onValueChanged -= _event;
    }
}
