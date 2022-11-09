using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Health : MonoBehaviour
{
    private int hp;
    public int HP { get { return hp; } set { if (hp < m_MaxHP) hp = value; } }

    [SerializeField] private int m_MaxHP;
    public int MaxHP { get { return m_MaxHP; } set { m_MaxHP = value; } }

    public Action E_TriggerDeath;

    [SerializeField] private VisualEffect m_DamageParticles;

    private void Awake()
    {
        SetHP(MaxHP);
    }

    public void SetHP(int _maxHP)
    {
        m_MaxHP = _maxHP;
        hp = _maxHP;
    }

    public void GetDamage(int _amount)
    {
        hp -= _amount;

        if (hp <= 0)
            E_TriggerDeath?.Invoke();

        if (transform.CompareTag("Light")) return;

        //m_DamageParticles.SetVector3("test", transform.position);
        //m_DamageParticles.Play();

        CameraManager.Instance.Shake();
    }
}
