using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Health : MonoBehaviour, IDamagable
{
    protected int hp;
    public int HP { get { return hp; } set { hp = value; } }

    [SerializeField] protected int m_MaxHP;
    public int MaxHP { get { return m_MaxHP; } set { m_MaxHP = value; } }

    public Action E_TriggerDeath;

    protected virtual void Awake()
    {
        SetHP();
    }

    private void SetHP()
    {
        hp = MaxHP;
    }

    public virtual void AddHP(int _amount)
    {
        HP += _amount;
    }

    public virtual void GetDamage(int _value, Vector3 _knockbackDir)
    {
        if (HP > 0)
            HP -= _value;

        if (HP <= 0)
            E_TriggerDeath?.Invoke();
    }
}
