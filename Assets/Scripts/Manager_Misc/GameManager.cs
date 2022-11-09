using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

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

    public PlayerInput PlayerInput;
    public List<Item> CollectedCollectibles;
    public List<CameraZoneSaveData> ZoneSaves;

    private void Awake()
    {
        Initialize();

        PlayerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {

    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        PlayerInput.actions.FindActionMap("Debug").Enable();
    }
#else
    private void OnEnable()
    {

    }
#endif

    #region Input
    public void OnSaveGame()
    {
        SaveSystem.Instance.e_SaveGame?.Invoke();
    }
    public void OnLoadGame()
    {
        SaveSystem.Instance.e_LoadGame?.Invoke();
    }
    #endregion
}
