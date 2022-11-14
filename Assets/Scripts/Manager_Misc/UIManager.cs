using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    #region Singleton

    private static UIManager instance;
    public static UIManager Instance { get { return instance; } }

    private void Initialize()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
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

    [Header("Sound")]
    [SerializeField] private AudioMixer m_Mixer;
    [SerializeField] private Slider m_MasterSlider;
    [SerializeField] private Slider m_SfxSlider;
    [SerializeField] private Slider m_MusicSlider;

    [Header("Level Transitions")]
    [SerializeField] private float m_TransitionSpeed;
    private bool transitionFlag;
    private Color color = new Color(0, 0, 0, 0);

    [Header("Objects")]
    [SerializeField] private GameObject m_MainMenuUI;
    [SerializeField] private GameObject m_InGameUI;
    [SerializeField] private GameObject m_PausePanel;
    [SerializeField] private GameObject m_OptionsPanel;
    [SerializeField] private GameObject[] m_MenuItems;
    [SerializeField] private Image m_TransitionImage;

    [Header("Cursor")]
    [SerializeField] private GameObject m_CursorObject;
    [SerializeField] private GameObject m_VFXCursorObject;
    [SerializeField] private Sprite m_MenuCursorTexture;
    [SerializeField] private Sprite m_InGameCursorTexture;
    [SerializeField] private SpriteRenderer m_CursorSprite;
    private Vector3 mousePos;

    private void Awake()
    {
        Initialize();

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
    private void Start()
    {
        Cursor.visible = false;

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
        {
            // set default sound settings
            m_MasterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
            m_SfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
            m_MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        }
    }

    private void Update()
    {
        if (this.GetComponent<Canvas>().worldCamera == null)
            this.GetComponent<Canvas>().worldCamera = Camera.main;

        mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos = new Vector3(mousePos.x, mousePos.y, 0);

        m_CursorObject.transform.position = mousePos;

        //#region InMenu
        //if (menuTransitionFlag)
        //{

        //}
        //#endregion

        //#region InGame
        //if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
        //{
        //    if (Input.GetButtonDown("Escape"))
        //    {
        //        TriggerPanel(m_PausePanel);
        //        if (m_OptionsPanel.active)
        //            TriggerPanel(m_OptionsPanel);
        //    }
        //}
        //#endregion


    }
    public void LoadGame(bool _newGameFlag)
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
            StartCoroutine(InGameTransition(1));
        else
            StartCoroutine(Transition(1, _newGameFlag));
    }
    private IEnumerator InGameTransition(int _sceneIndex)
    {
        transitionFlag = true;
        Time.timeScale = 0f;
        if (m_PausePanel.active)
            m_PausePanel.SetActive(false);

        while (m_TransitionImage.color.a < 1)
        {
            color.a += Time.unscaledDeltaTime / m_TransitionSpeed;
            m_TransitionImage.color = color;
            yield return null;
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(_sceneIndex);
        m_MainMenuUI.SetActive(true);
        m_InGameUI.SetActive(false);

        m_CursorSprite.sprite = m_MenuCursorTexture;
        m_VFXCursorObject.SetActive(true);
    }

    private IEnumerator Transition(int _sceneIndex, bool _newGameFlag)
    {
        //this.GetComponent<Canvas>().enabled = false;

        // while screen is fading to black do
        while (m_TransitionImage.color.a < 1)
        {
            color.a += Time.unscaledDeltaTime / m_TransitionSpeed;
            m_TransitionImage.color = color;
            yield return null;
        }

        AsyncOperation load = SceneManager.LoadSceneAsync(_sceneIndex);

        while (!load.isDone)
        {
            yield return null;
        }

        if (!_newGameFlag)
            GameManager.Instance.OnLoadGame();

        m_InGameUI.SetActive(true);
        m_MainMenuUI.SetActive(false);
        m_TransitionImage.color = new Color(0, 0, 0, 0);

        m_CursorSprite.sprite = m_InGameCursorTexture;
        m_VFXCursorObject.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    public void TriggerPanel(GameObject _panel)
    {
        for (int i = 0; i < m_MenuItems.Length; i++)
        {
            m_MenuItems[i].SetActive(!m_MenuItems[i].active);
        }
        _panel.SetActive(!_panel.active);

        if (_panel == m_PausePanel)
        {
            if (!transitionFlag)
            {
                if (m_PausePanel.active)
                    Time.timeScale = 0f;
                else if (!m_PausePanel.active)
                    Time.timeScale = 1f;
            }
        }
    }

    #region Sound Management
    public void ChangeMasterVolume(float _volume)
    {
        m_Mixer.SetFloat("MasterVolume", Mathf.Log10(_volume) * 20f);
        SaveVolumeLevel(m_MasterSlider, "MasterVolume");
    }
    public void ChangeSFXVolume(float _volume)
    {
        m_Mixer.SetFloat("SFXVolume", Mathf.Log10(_volume) * 20f);
        SaveVolumeLevel(m_SfxSlider, "SFXVolume");
    }
    public void ChangeMusicVolume(float _volume)
    {
        m_Mixer.SetFloat("MusicVolume", Mathf.Log10(_volume) * 20f);
        SaveVolumeLevel(m_MusicSlider, "MusicVolume");
    }

    private void SaveVolumeLevel(Slider _slider, string _prefsName)
    {
        float sliderValue = _slider.value;
        PlayerPrefs.SetFloat(_prefsName, sliderValue);
    }
    #endregion

    private void OnDestroy()
    {
        m_PlayerHealth.UnRegister(UpdateHealthBar);

        Terminate();
    }
}
