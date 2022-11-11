using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D m_Rb;
    [SerializeField] private int m_Damage = 1;
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private float shootForce;
    [SerializeField] private LayerMask m_ObstacleLayer;
    private LayerMask ignoredLayer;
    public float Cooldown;

    public void Launch(Vector2 _dir, float _duration, LayerMask _ignoredLayer)
    {
        ignoredLayer = _ignoredLayer;
        _dir.Normalize();
        m_Rb.velocity = _dir * shootForce;
        m_SpriteRenderer.transform.right = m_Rb.velocity;
        Invoke("DeleteProjectile", _duration);
    }
    private void DeleteProjectile()
    {
        Destroy(this.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D _other)
    {
        IDamagable dmg = _other.GetComponent<IDamagable>();

        if ((1 << _other.gameObject.layer) == m_ObstacleLayer)
        {
            SoundManager.Instance.HitObjectSound.Play();
            Destroy(this.gameObject);
            return;
        }

        if (dmg == null || (1 << _other.gameObject.layer) == ignoredLayer) return;

        dmg.GetDamage(m_Damage, m_Rb.velocity);

        Destroy(this.gameObject);
    }
}
