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

    public string saveFilePath;

    [SerializeField] private ScriptableInt m_PlayerHealth;

    public Action e_SaveGame;
    public Action e_LoadGame;

    private void Awake()
    {
        Initialize();

        //saveFilePath = Application.dataPath + "/save.json";
        saveFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/StealthyVengeanceSaveFile.json";

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
            PlayerHealth = m_PlayerHealth.Value,
            AmountOfJumps = PlayerController.Instance.AmountOfJumps,
            AllowWallClimb = PlayerController.Instance.AllowWallClimb,
            AllowWallHang = PlayerController.Instance.AllowWallHang,
            AllowWallHops = PlayerController.Instance.AllowWallHops,
            AllowDashing = PlayerController.Instance.AllowDashing,
            AllowJumping = PlayerController.Instance.AllowJumping,
            AllowMoving = PlayerController.Instance.AllowMoving,
            //CollectedCollectibles = GameManager.Instance.CollectedCollectibles,
            CameraZones = GameManager.Instance.ZoneSaves,
            FakeLights = GameManager.Instance.FakeLightsSaves,
        };

        return data;
    }
    private void ApplyAllSaveData(SaveObject _data)
    {
        PlayerController.Instance.transform.position = _data.PlayerPosition;
        m_PlayerHealth.Value = _data.PlayerHealth;
        PlayerController.Instance.AmountOfJumps = _data.AmountOfJumps;
        PlayerController.Instance.AllowWallClimb = _data.AllowWallClimb;
        PlayerController.Instance.AllowWallHang = _data.AllowWallHang;
        PlayerController.Instance.AllowWallHops = _data.AllowWallHops;
        PlayerController.Instance.AllowDashing = _data.AllowDashing;
        PlayerController.Instance.AllowJumping = _data.AllowJumping;
        PlayerController.Instance.AllowMoving = _data.AllowMoving;
        //GameManager.Instance.CollectedCollectibles = _data.CollectedCollectibles;

        for (int i = 0; i < ZoneManager.Instance.Zones.Length; i++)
        {
            for (int k = 0; k < _data.CameraZones.Count; k++)
            {
                if (ZoneManager.Instance.Zones[i].ID == _data.CameraZones[k].ZoneID)
                    ZoneManager.Instance.Zones[i].WasVisited = _data.CameraZones[k].WasVisited;

            }
        }

        GameManager.Instance.ZoneSaves = _data.CameraZones;

        for (int i = 0; i < GameManager.Instance.FakeLights.Length; i++)
        {
            for (int k = 0; k < _data.FakeLights.Count; k++)
            {
                if (GameManager.Instance.FakeLights[i].ID == _data.FakeLights[k].LightID)
                    GameManager.Instance.FakeLights[i].Dead = _data.FakeLights[k].IsDead;
            }
        }

        GameManager.Instance.FakeLightsSaves = _data.FakeLights;
    }
}
public class SaveObject
{
    // Variables that should be saved
    public Vector2 PlayerPosition;
    public int PlayerHealth;
    public int AmountOfJumps;
    public bool AllowWallClimb;
    public bool AllowWallHang;
    public bool AllowWallHops;
    public bool AllowDashing;
    public bool AllowJumping;
    public bool AllowMoving;
    //public List<Item> CollectedCollectibles;
    public List<CameraZoneSaveData> CameraZones;
    public List<FakeLightSaveData> FakeLights;
}
[System.Serializable]
public struct CameraZoneSaveData
{
    public ushort ZoneID;
    public bool WasVisited;
    public CameraZoneSaveData(ushort _ID, bool _wasVisited)
    {
        ZoneID = _ID;
        WasVisited = _wasVisited;
    }
}
[System.Serializable]
public struct FakeLightSaveData
{
    public ushort LightID;
    public bool IsDead;
    public FakeLightSaveData(ushort _ID, bool _isDead)
    {
        LightID = _ID;
        IsDead = _isDead;
    }
}
