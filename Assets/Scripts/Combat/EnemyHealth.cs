using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyHealth : Health
{
    [SerializeField] private Rigidbody2D m_Rb;
    [SerializeField] private float m_KnockbackForce;
    [SerializeField] private VisualEffect m_BloodParticles;
    [SerializeField] private VisualEffect m_DespawnParticles;

    public override void GetDamage(int _value, Vector3 _knockbackDir)
    {
        CameraManager.Instance.Shake();

        float defaultX = Mathf.Sign(_knockbackDir.x);
        if (defaultX > 0)
            defaultX = 1;
        else if (defaultX < 0)
            defaultX = -1;

        // play blood particles
        m_BloodParticles.SetVector3("Force", new Vector3((_knockbackDir.normalized.x + defaultX) * 5f, 2f, 2f));
        m_BloodParticles.Play();

        // play sound
        SoundManager.Instance.PlaySound(SoundManager.Instance.HitHumanoidSound);


        // Do knockback
        m_Rb.AddForce(_knockbackDir * m_KnockbackForce, ForceMode2D.Impulse);


        if (base.hp - 1 <= 0)
        {
            m_BloodParticles.transform.SetParent(null);
            m_DespawnParticles.transform.SetParent(null);
            m_DespawnParticles.Play();
        }

        base.GetDamage(_value, _knockbackDir);
    }
}
