using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D m_Rb;
    [SerializeField] private int m_Damage;
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    private string ignoredTag;
    public void Launch(Vector2 _dir, float _force, float _duration, string _ignoreTag)
    {
        ignoredTag = _ignoreTag;
        _dir.Normalize();
        m_Rb.velocity = _dir * _force;
        m_SpriteRenderer.transform.right = m_Rb.velocity;  
        Invoke("DeleteProjectile", _duration);
    }
    private void DeleteProjectile()
    {
        Destroy(this.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag(ignoredTag)) return;
        if (_other.CompareTag("Projectile")) return;

        // Do damage!
        _other.GetComponent<Health>()?.GetDamage(m_Damage);


        Destroy(this.gameObject);
    }
}
