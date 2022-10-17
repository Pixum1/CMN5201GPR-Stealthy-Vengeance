using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    #region Singleton
    private static SaveSystem instance;
    public static SaveSystem Instance { get { return instance; } }

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

    private string saveFilePath;

    public Action e_SaveGame;
    public Action e_LoadGame;

    private void Awake()
    {
        Initialize();

        saveFilePath = Application.dataPath + "/save.json";

        e_SaveGame = Save;
        e_LoadGame = Load;
    }

    private void Save()
    {
        try
        {
            SaveObject saveState = SaveAllDataToObject();
            string json = JsonUtility.ToJson(saveState);

            File.WriteAllText(saveFilePath, json);

            Debug.LogWarning("All data has been saved to " + saveFilePath);
        }
        catch (Exception e)
        {
            Debug.LogWarning("There was an error while saving!\n Error message: " + e.Message);
        }
    }
    private void Load()
    {
        try
        {
            if (File.Exists(saveFilePath))
            {
                string saveString = File.ReadAllText(saveFilePath);

                SaveObject data = JsonUtility.FromJson<SaveObject>(saveString);

                ApplyAllSaveData(data);

                Debug.LogWarning("All data has been loaded from " + saveFilePath);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("There was an error while loading!\n Error message: " + e.Message);
        }
    }

    private SaveObject SaveAllDataToObject()
    {
        SaveObject data = new SaveObject()
        {
            PlayerPosition = PlayerController.Instance.transform.position,
            PlayerHealth = PlayerController.Instance.Health.HP,
            AmountOfJumps = PlayerController.Instance.AmountOfJumps,
            CollectedCollectibles = GameManager.Instance.CollectedCollectibles,
        };

        return data;
    }
    private void ApplyAllSaveData(SaveObject _data)
    {
        PlayerController.Instance.transform.position = _data.PlayerPosition;
        PlayerController.Instance.Health.HP = _data.PlayerHealth;
        PlayerController.Instance.AmountOfJumps = _data.AmountOfJumps;
        GameManager.Instance.CollectedCollectibles = _data.CollectedCollectibles;
    }
}
public class SaveObject
{
    // Variables that should be saved
    public Vector2 PlayerPosition;
    public int PlayerHealth;
    public int AmountOfJumps;
    public List<Collectible> CollectedCollectibles;
}
