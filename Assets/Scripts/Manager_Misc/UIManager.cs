using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

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
    private Color color = new Color(0, 0, 0, 0);

    [Header("Objects")]
    [SerializeField] private GameObject m_MainMenuUI;
    [SerializeField] private GameObject m_InGameUI;
    [SerializeField] private GameObject m_PausePanel;
    [SerializeField] private GameObject m_OptionsPanel;
    [SerializeField] private GameObject[] m_MenuItems;
    [SerializeField] private Image m_TransitionImage;
    [SerializeField] private SpriteRenderer m_CampfireTransitionImage;
    [SerializeField] private GameObject m_BlurredBackground;
    [SerializeField] private GameObject m_DeathPanel;
    [SerializeField] private GameObject m_WinPanel;

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

    public void OnPause(CallbackContext _ctx)
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(1))
            OpenPausePanel();
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


    }
    public void LoadMenu()
    {
        GameManager.Instance.PlayerInput.actions.actionMaps[0].Disable();
        StartCoroutine(InGameTransition());
    }
    public void LoadGame(bool _newGameFlag)
    {
        GameManager.Instance.PlayerInput.actions.actionMaps[0].Disable();
        StartCoroutine(Transition(1, _newGameFlag));
    }
    private IEnumerator InGameTransition()
    {
        Time.timeScale = 0f;
        if (m_PausePanel.activeSelf)
            m_PausePanel.SetActive(false);

        

        while (m_TransitionImage.color.a < 1)
        {
            color.a += Time.unscaledDeltaTime / m_TransitionSpeed;
            m_TransitionImage.color = color;
            yield return null;
        }

        AsyncOperation load = SceneManager.LoadSceneAsync(0);
        while (!load.isDone)
        {
            yield return null;
        }

        Time.timeScale = 1f;

        m_InGameUI.SetActive(false);

        #region Change Cursor
        m_CursorSprite.sprite = m_MenuCursorTexture;
        m_VFXCursorObject.SetActive(true);
        #endregion

        color = new Color(0, 0, 0, 1);

        while (m_TransitionImage.color.a > 0)
        {
            color.a -= Time.unscaledDeltaTime / m_TransitionSpeed;
            m_TransitionImage.color = color;
            yield return null;
        }
        color = new Color(0, 0, 0, 0);
        m_MainMenuUI.SetActive(true);

        GameManager.Instance.PlayerInput.actions.actionMaps[0].Enable();

    }

    private IEnumerator Transition(int _sceneIndex, bool _newGameFlag)
    {
        CloseDeathPanel();

        // while screen is fading to black do
        while (m_TransitionImage.color.a < 1)
        {
            color.a += Time.unscaledDeltaTime / m_TransitionSpeed;
            m_TransitionImage.color = color;
            yield return null;
        }

        color = new Color(0, 0, 0, 0);

        AsyncOperation load = SceneManager.LoadSceneAsync(_sceneIndex);
        load.allowSceneActivation = false;

        while (load.progress < .9f)
        {
            yield return null;
        }

        while (m_CampfireTransitionImage.color.a < 1)
        {
            color.a += Time.unscaledDeltaTime / m_TransitionSpeed;
            m_CampfireTransitionImage.color = color;
            yield return null;
        }

        color = new Color(0, 0, 0, 1);

        load.allowSceneActivation = true;

        while (!load.isDone)
        {
            yield return null;
        }

        m_InGameUI.SetActive(true);
        m_MainMenuUI.SetActive(false);

        #region Change cursor
        m_CursorSprite.sprite = m_InGameCursorTexture;
        m_VFXCursorObject.SetActive(false);
        #endregion

        if (!_newGameFlag)
            GameManager.Instance.OnLoadGame();

        yield return new WaitForSecondsRealtime(1f);

        m_TransitionImage.color = color;
        m_CampfireTransitionImage.color = new Color(0, 0, 0, 0);

        while (m_TransitionImage.color.a > 0)
        {
            color.a -= Time.unscaledDeltaTime / m_TransitionSpeed;
            m_TransitionImage.color = color;
            yield return null;
        }
        m_TransitionImage.color = new Color(0, 0, 0, 0);

        color = new Color(0, 0, 0, 0);


        GameManager.Instance.PlayerInput.actions.actionMaps[0].Enable();
    }
    public void OpenDeathPanel()
    {
        for (int i = 0; i < m_MenuItems.Length; i++)
        {
            m_MenuItems[i].SetActive(false);
        }
        m_DeathPanel.SetActive(true);
        m_BlurredBackground.SetActive(true);
    }
    private void CloseDeathPanel()
    {
        for (int i = 0; i < m_MenuItems.Length; i++)
        {
            m_MenuItems[i].SetActive(false);
        }
        m_DeathPanel.SetActive(false);
        m_BlurredBackground.SetActive(false);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void TriggerPanel(GameObject _panel)
    {
        for (int i = 0; i < m_MenuItems.Length; i++)
        {
            m_MenuItems[i].SetActive(false);
        }
        _panel.SetActive(true);
    }
    public void OpenPausePanel()
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(1)) return;
        if (m_DeathPanel.activeInHierarchy) return;

        for (int i = 0; i < m_MenuItems.Length; i++)
        {
            m_MenuItems[i].SetActive(false);
        }
        m_PausePanel.SetActive(true);
        m_BlurredBackground.SetActive(true);
        Time.timeScale = 0;
    }
    public void ClosePausePanel()
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(1)) return;

        for (int i = 0; i < m_MenuItems.Length; i++)
        {
            m_MenuItems[i].SetActive(false);
        }
        m_PausePanel.SetActive(false);
        m_BlurredBackground.SetActive(false);
        Time.timeScale = 1;
    }
    public void TriggerOptionsPanel()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
            TriggerPanel(m_MainMenuUI);
        else
            OpenPausePanel();
    }

    public void ShowWinPanel()
    {
        for (int i = 0; i < m_MenuItems.Length; i++)
        {
            m_MenuItems[i].SetActive(false);
        }
        m_WinPanel.SetActive(true);
        m_BlurredBackground.SetActive(true);
    }
    public void CloseWinPanel()
    {
        for (int i = 0; i < m_MenuItems.Length; i++)
        {
            m_MenuItems[i].SetActive(false);
        }
        m_WinPanel.SetActive(false);
        m_BlurredBackground.SetActive(false);
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
