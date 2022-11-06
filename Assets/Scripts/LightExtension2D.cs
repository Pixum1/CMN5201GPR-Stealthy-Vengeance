using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D), typeof(Health))]
public class LightExtension2D : MonoBehaviour
{
    Light2D light;
    Health health;
    SpriteRenderer sprRend;

    Sprite originalSprite;
    float originalIntensity;
    float offtimer;
    bool dead;

    [SerializeField] Sprite m_DeadSprite;
    [SerializeField] float m_MaxTime = 25f;

    private void Awake()
    {
        light = GetComponent<Light2D>();
        health = GetComponent<Health>();
        sprRend = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        health.E_TriggerDeath += DestroyLamp;
        ZoneManager.Instance.E_ChangedZone += RegenerateLamp;

        originalSprite = sprRend.sprite;
        originalIntensity = light.intensity;
        
        offtimer = Random.Range(0, m_MaxTime);
    }
    private void Update()
    {
        if (dead) return;

        if (offtimer <= 0)
        {
            light.intensity = 0;
            offtimer = Random.Range(0, m_MaxTime);
        }
        else
        {
            light.intensity = originalIntensity;
        }
        offtimer -= Time.deltaTime;
    }
    private void RegenerateLamp()
    {
        dead = false;

        // change sprite
        sprRend.sprite = m_DeadSprite;

        // turn on light
        light.intensity = originalIntensity;
    }
    private void DestroyLamp()
    {
        dead = true;

        // change sprite
        sprRend.sprite = originalSprite;

        // turn off light
        light.intensity = 0f;
    }
}
