using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D m_Rb;
    public void Launch(Vector2 _dir, float _force, float _duration)
    {
        _dir.Normalize();
        m_Rb.velocity = _dir * _force;
        Invoke("DeleteProjectile", _duration);
    }
    private void DeleteProjectile()
    {
        Destroy(this.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player")) return;
        if (_other.CompareTag("Projectile")) return;

        // Do damage!

        Destroy(this.gameObject);
    }
}
