using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    #region Singleton
    private static ZoneManager instance;
    public static ZoneManager Instance { get { return instance; } }

    private void Initialize()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
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
    [SerializeField] private Vector2 standardSize = new Vector2(40, 22);
    [SerializeField] private GameObject m_MapVisuals;

    public CameraZone[] Zones;
    public LayerMask PlayerLayer;
    public CameraZone CurrentActiveZone;
    //public ScriptableEvent StartEncounterEvent;
    //public ScriptableEvent EndEncounterEvent;
    public Action E_ChangedZone;

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        Zones = FindObjectsOfType<CameraZone>();
    }
    private void Update()
    {
        for (int i = 0; i < Zones.Length; i++)
        {
            Zones[i].UpdateRoom();
            //prevent the bug where the player can stand in two rooms at once
            if (Zones[i].IsActive && CurrentActiveZone != Zones[i])
            {
                CurrentActiveZone = Zones[i];
                E_ChangedZone?.Invoke();
            }
        }
    }
    /// <summary>
    /// Creates a new CameraZone and returns it.
    /// </summary>
    /// <returns>Instance of CameraZone</returns>
    public GameObject CreateZone()
    {
        // Get new Zone ID
        ushort id = 0;
        CameraZone[] zones = FindObjectsOfType<CameraZone>();

        for (int i = 0; i < zones.Length; i++)
        {
            if (id <= zones[i].ID)
            {
                id = zones[i].ID;
                id++;
            }
        }

        // Create a new zone
        GameObject zone = new GameObject("CameraZone");
        zone.transform.SetParent(transform);
        zone.transform.localScale = standardSize;
        zone.AddComponent<BoxCollider>().isTrigger = true;
        CameraZone z = zone.AddComponent<CameraZone>();
        z.ID = id;

        // Create visuals for the minimap
        GameObject visual = Instantiate(m_MapVisuals);
        visual.transform.SetParent(zone.transform);
        visual.name = "MapVisual";
        z.MapVisual = visual.GetComponent<SpriteRenderer>();
        z.MapVisual.drawMode = SpriteDrawMode.Sliced;
        z.MapVisual.size = zone.transform.localScale;

        return zone;
    }

    private void OnDestroy()
    {
        Terminate();
    }
}
