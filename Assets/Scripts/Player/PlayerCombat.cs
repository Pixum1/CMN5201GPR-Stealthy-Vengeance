using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerCombat : MonoBehaviour
{
    private Camera mainCam;
    private Mouse mouse;
    private float timer;

    [SerializeField] private Projectile m_Projectile;
    [SerializeField] private float m_ProjectileRegenerationTime;
    [SerializeField] private float m_BulletLifeTime;
    [SerializeField] private SpriteRenderer[] m_FloatingProjectiles;

    private void Start()
    {
        mouse = Mouse.current;
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (timer > -1)
            timer -= Time.deltaTime;
    }

    public void OnFire(CallbackContext _ctx)
    {
        if (timer <= 0)
        {
            Vector3 mousePos = mainCam.ScreenToWorldPoint(mouse.position.ReadValue());
            // Player facing left -> shoot from the left
            if (mousePos.x - transform.position.x < 0)
            {
                for (int i = 0; i < m_FloatingProjectiles.Length; i++)
                {
                    if (!m_FloatingProjectiles[i].enabled) continue;

                    Vector3 pos = new Vector3(transform.position.x - .5f, transform.position.y);
                    Vector3 dir = mousePos - pos;

                    StartCoroutine(TriggerFloatingPrjectile(m_FloatingProjectiles[i]));

                    Projectile p = Instantiate(m_Projectile, pos, Quaternion.identity);
                    p.Launch(dir, m_BulletLifeTime, LayerMask.GetMask("Player"));

                    timer = m_Projectile.Cooldown;
                    break;
                }
            }
            // Player facing right -> shoot to the right
            else
            {
                for (int i = m_FloatingProjectiles.Length-1; i >= 0; i--)
                {
                    if (!m_FloatingProjectiles[i].enabled) continue;

                    Vector3 pos = new Vector3(transform.position.x + .5f, transform.position.y);
                    Vector3 dir = mousePos - pos;

                    StartCoroutine(TriggerFloatingPrjectile(m_FloatingProjectiles[i]));

                    Projectile p = Instantiate(m_Projectile, pos, Quaternion.identity);
                    p.Launch(dir, m_BulletLifeTime, LayerMask.GetMask("Player"));

                    timer = m_Projectile.Cooldown;
                    break;
                }
            }
        }
    }

    private IEnumerator TriggerFloatingPrjectile(SpriteRenderer _floatingProjectile)
    {
        _floatingProjectile.enabled = false;
        yield return new WaitForSeconds(m_ProjectileRegenerationTime);
        _floatingProjectile.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere((Vector2)mainCam.ScreenToWorldPoint(mouse.position.ReadValue()), .25f);
        }
    }
}
