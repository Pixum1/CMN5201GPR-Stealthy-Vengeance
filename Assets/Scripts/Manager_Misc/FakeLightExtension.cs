using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class FakeLightExtension : MonoBehaviour
{
    public ushort ID = 0;

    private float offtimer;
    private bool dead;
    public bool Dead
    {
        get { return dead; }

        set
        {
            if (value == dead) return;

            dead = value;

            if (value == true)
            {
                DestroyLamp();
                if (GameManager.Instance != null)
                    for (int i = 0; i < GameManager.Instance.FakeLightsSaves.Count; i++)
                    {
                        // Find this zone in the GameManagers zone list
                        if (GameManager.Instance.FakeLightsSaves[i].LightID != ID) continue;

                        // Create a new data object at the location in the list
                        GameManager.Instance.FakeLightsSaves[i] = new FakeLightSaveData(ID, true);
                    }
            }
            else
                RegenerateLamp();
        }
    }

    [SerializeField] private bool m_FlickerFlag;
    [SerializeField] private GameObject m_Lights;
    [SerializeField] private BoxCollider2D m_Collider;
    [SerializeField] private Health health;
    [SerializeField] private SpriteRenderer sprRend;
    [SerializeField] private Sprite m_AliveSprite;
    [SerializeField] private Sprite m_DeadSprite;
    [SerializeField] private float m_MaxTime = 25f;

    [SerializeField] private float m_MinSize;
    [SerializeField] private float m_MaxSize;

    private void Start()
    {
        GameManager.Instance.FakeLightsSaves.Add(new FakeLightSaveData(ID, Dead));

        health.E_TriggerDeath += DestroyLamp;

        // ZoneManager.Instance.E_ChangedZone += RegenerateLamp;

        // set original size
        m_Lights.SetActive(true);

        offtimer = Random.Range(0, m_MaxTime);
    }
    private void Update()
    {
        if (Dead) return;
        if (!m_FlickerFlag) return;

        if (offtimer <= 0)
        {
            m_Lights.transform.localScale = Vector3.one * Random.Range(m_MinSize, m_MaxSize);
            offtimer = Random.Range(0, m_MaxTime);
        }

        offtimer -= Time.deltaTime;
    }
    private void RegenerateLamp()
    {
        Dead = false;

        // turn on collider
        m_Collider.enabled = true;

        // change sprite
        sprRend.sprite = m_AliveSprite;

        // reset light size
        m_Lights.SetActive(true);
    }
    private void DestroyLamp()
    {
        Dead = true;

        // turn off collider
        m_Collider.enabled = false;

        // change sprite
        sprRend.sprite = m_DeadSprite;

        // set light size to 0
        m_Lights.SetActive(false);
    }
}
