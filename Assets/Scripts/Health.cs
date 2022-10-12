using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private int hp;
    public int HP { get { return hp; } set { hp = value; } }

    [SerializeField] private int m_MaxHP;
    public int MaxHP { get { return m_MaxHP; } set { m_MaxHP = value; } }

    public Action E_TriggerDeath;

    private void Start()
    {
        hp = m_MaxHP;
    }

    public void GetDamage(int _amount)
    {
        hp -= _amount;

        if(hp<=0)
            E_TriggerDeath?.Invoke();
    }
}
