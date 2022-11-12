using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Singleton

    private UIManager instance;
    public UIManager Instance { get { return instance; } }

    private void Initialize()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Terminate()
    {
        if (this == Instance)
        {
            instance = null;
        }
    }

    #endregion

    [Header("Healthbar")]
    [SerializeField] private Image[] m_HealthContainer;
    [SerializeField] private GameObject m_Healthbar;
    [SerializeField] private ScriptableInt m_PlayerHealth;
    [SerializeField] private Sprite m_FullHeart;
    [SerializeField] private Sprite m_HalfHeart;

    private void Awake()
    {
        m_PlayerHealth.Register(UpdateHealthBar);
    }

    private void UpdateHealthBar()
    {
        for (int i = 1; i < m_HealthContainer.Length + 1; i++)
        {
            // enables healthcontainer
            m_HealthContainer[i - 1].enabled = true;

            // check if playerhealth is less or equal than the current healthcontainer index doubled
            if (i + i <= m_PlayerHealth.Value)
                m_HealthContainer[i - 1].sprite = m_FullHeart;
            // check if playerhealth is less or equal than the current healthcontainer index doubled MINUS one
            else if (i + i - 1 <= m_PlayerHealth.Value)
                m_HealthContainer[i - 1].sprite = m_HalfHeart;
            // if none of the above: disable healthcontainer
            else
                m_HealthContainer[i - 1].enabled = false;
        }
    }

    private void OnDestroy()
    {
        m_PlayerHealth.UnRegister(UpdateHealthBar);
    }
}
