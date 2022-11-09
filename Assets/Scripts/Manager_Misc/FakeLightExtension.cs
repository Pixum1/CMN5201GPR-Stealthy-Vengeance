using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class FakeLightExtension : MonoBehaviour
{
    Health health;
    SpriteRenderer sprRend;

    float offtimer;
    bool dead;

    [SerializeField] bool m_FlickerFlag;
    [SerializeField] GameObject m_Lights;
    [SerializeField] Sprite m_AliveSprite;
    [SerializeField] Sprite m_DeadSprite;
    [SerializeField] float m_MaxTime = 25f;

    [SerializeField] float m_MinSize;
    [SerializeField] float m_MaxSize;

    private void Awake()
    {
        health = GetComponent<Health>();
        sprRend = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        health.E_TriggerDeath += DestroyLamp;
        ZoneManager.Instance.E_ChangedZone += RegenerateLamp;

        // set original size

        offtimer = Random.Range(0, m_MaxTime);
    }
    private void Update()
    {
        if (dead) return;
        if (!m_FlickerFlag) return;

        if (offtimer <= 0)
        {
            m_Lights.transform.localScale = Vector3.one * Random.Range(m_MinSize, m_MaxSize);
            offtimer = Random.Range(0, m_MaxTime);
        }

        offtimer -= Time.deltaTime;

        //if (offtimer <= 0)
        //{
        //    // set light size to 0
        //    offtimer = Random.Range(0, m_MaxTime);
        //}
        //else
        //{
        //    // reset light size
        //}
        //offtimer -= Time.deltaTime;
    }
    private void RegenerateLamp()
    {
        dead = false;

        // change sprite
        sprRend.sprite = m_AliveSprite;

        // reset light size
        m_Lights.SetActive(true);
    }
    private void DestroyLamp()
    {
        dead = true;

        // change sprite
        sprRend.sprite = m_DeadSprite;

        // set light size to 0
        m_Lights.SetActive(false);
    }
}
