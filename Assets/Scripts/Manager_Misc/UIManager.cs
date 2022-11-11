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
    [SerializeField] private ScriptableInt m_PlayerHealth;
    [SerializeField] private Sprite m_FullHeart;
    [SerializeField] private Sprite m_HalfHeart;

    private void Awake()
    {
        m_PlayerHealth.Register(UpdateHealthBar);
    }
    private void UpdateHealthBar()
    {
        for (int i = m_PlayerHealth.Value; i > 0; i--)
        {

            // i / 2 - 1??????
            int c = Mathf.FloorToInt(i / 2);

            Debug.Log(c);
        }

        //for (int i = 0; i < m_HealthContainer.Length; i++)
        //{
        //    if ((m_PlayerHealth.Value - 1) / 2f >= i)
        //    {
        //        m_HealthContainer[i].enabled = true;
        //        continue;
        //    }

        //    m_HealthContainer[i].enabled = false;
        //}
    }
    private void OnDestroy()
    {
        m_PlayerHealth.UnRegister(UpdateHealthBar);
    }
}
