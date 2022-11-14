using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    [HideInInspector] public List<Item> CollectedCollectibles;
    [HideInInspector] public List<CameraZoneSaveData> ZoneSaves;
    [HideInInspector] public List<FakeLightSaveData> FakeLightsSaves;
    [HideInInspector] public FakeLightExtension[] FakeLights;

    private void OnValidate()
    {
        FakeLightExtension[] lights = FindObjectsOfType<FakeLightExtension>();

        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i].ID != 0) continue;

            for (int k = 0; k < lights.Length; k++)
            {
                if (lights[i].ID <= lights[k].ID)
                    lights[i].ID = (ushort)(lights[k].ID + 1);
            }
        }
    }

    private void Awake()
    {
        Initialize();

        FakeLightsSaves = new List<FakeLightSaveData>();
    }

    private void Start()
    {
        // In Game
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
            FakeLights = FindObjectsOfType<FakeLightExtension>();
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

    private void OnDestroy()
    {
        Terminate();
    }
}
