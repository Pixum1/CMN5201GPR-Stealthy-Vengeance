using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WinningItem : Item
{
    [SerializeField] private VisualEffect[] m_WinParticles;
    public override void OnCollect()
    {
        // Disable Player input
        GameManager.Instance.PlayerInput.actions.actionMaps[0].Disable();

        // Play Particles etc.
        for (int i = 0; i < m_WinParticles.Length; i++)
        {
            m_WinParticles[i].Play();
        }

        // Show Win screen
        UIManager.Instance.ShowWinPanel();
    }
}
