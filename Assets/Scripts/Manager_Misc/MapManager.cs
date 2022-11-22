using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using System;
using UnityEngine.Events;

public class MapManager : MonoBehaviour
{
    #region Singleton
    private static MapManager instance;
    public static MapManager Instance { get { return instance; } }

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
    [SerializeField] private Color m_VisitedColor;
    [SerializeField] private Color m_NormalColor;
    [SerializeField] private Color m_CurrentZoneColor;
    [SerializeField, Range(0f, 1f)] private float m_Transparency;

    [Header("Mini Map")]
    [SerializeField] private Image m_MiniMapImage;
    [SerializeField] private float m_NormalMiniMapSize;
    private bool minimapOpenFlag;

    [Header("Maxi Map")]
    [SerializeField] private Image m_MaxiMapImage;
    [SerializeField] private float m_NormalMaxiMapSize;
    [SerializeField] private float m_ZoomIntensity;
    [SerializeField] private float m_MinimumZoom;
    [SerializeField] private float m_MaximumZoom;
    private float maxiMapZoom;
    private bool maximapOpenFlag;
    private Vector2 maximapCoords = Vector2.zero;

    private Vector2 mapBoundsMax = Vector2.zero;
    private Vector2 mapBoundsMin = Vector2.zero;
    private Camera cam;

    private void Awake()
    {
        Initialize();

        GameManager.Instance.PlayerInput.actions.actionMaps[0].FindAction("MiniMap").started += OnMiniMap;
        GameManager.Instance.PlayerInput.actions.actionMaps[0].FindAction("MiniMap").canceled += OnMiniMap;

        GameManager.Instance.PlayerInput.actions.actionMaps[0].FindAction("MaxiMap").performed += OnMaxiMap;
        GameManager.Instance.PlayerInput.actions.actionMaps[0].FindAction("MaxiMap").canceled += OnMaxiMap;

        GameManager.Instance.PlayerInput.actions.actionMaps[3].FindAction("Move").performed += OnMove;
        GameManager.Instance.PlayerInput.actions.actionMaps[3].FindAction("Move").canceled += OnMove;

        GameManager.Instance.PlayerInput.actions.actionMaps[3].FindAction("Close").performed += OnClose;
        GameManager.Instance.PlayerInput.actions.actionMaps[3].FindAction("Close").canceled += OnClose;

        GameManager.Instance.PlayerInput.actions.actionMaps[3].FindAction("Zoom").performed += OnZoom;
        GameManager.Instance.PlayerInput.actions.actionMaps[3].FindAction("Zoom").canceled += OnZoom;
    }

    public void RemoveBindings()
    {
        GameManager.Instance.PlayerInput.actions.actionMaps[0].FindAction("MiniMap").started -= OnMiniMap;
        GameManager.Instance.PlayerInput.actions.actionMaps[0].FindAction("MiniMap").canceled -= OnMiniMap;

        GameManager.Instance.PlayerInput.actions.actionMaps[0].FindAction("MaxiMap").performed -= OnMaxiMap;
        GameManager.Instance.PlayerInput.actions.actionMaps[0].FindAction("MaxiMap").canceled -= OnMaxiMap;

        GameManager.Instance.PlayerInput.actions.actionMaps[3].FindAction("Move").performed -= OnMove;
        GameManager.Instance.PlayerInput.actions.actionMaps[3].FindAction("Move").canceled -= OnMove;

        GameManager.Instance.PlayerInput.actions.actionMaps[3].FindAction("Close").performed -= OnClose;
        GameManager.Instance.PlayerInput.actions.actionMaps[3].FindAction("Close").canceled -= OnClose;

        GameManager.Instance.PlayerInput.actions.actionMaps[3].FindAction("Zoom").performed -= OnZoom;
        GameManager.Instance.PlayerInput.actions.actionMaps[3].FindAction("Zoom").canceled -= OnZoom;
    }

    private void Start()
    {
        CalculateBounds();
        cam = GetComponent<Camera>();

    }
    private void Update()
    {
        if (minimapOpenFlag)
            UpdateZones();
        if (maximapOpenFlag)
            UpdateMaxiMapCoords();
    }

    #region Input
    public void OnMiniMap(CallbackContext _ctx)
    {
        OpenMiniMap();

        if (_ctx.canceled)
            CloseMiniMap();
    }
    public void OnMaxiMap(CallbackContext _ctx)
    {
        if(mapBoundsMax == Vector2.zero && mapBoundsMin == Vector2.zero)
            CalculateBounds();

        OpenMaxiMap();
    }
    public void OnClose(CallbackContext _ctx)
    {
        CloseMaxiMap();
    }
    public void OnMove(CallbackContext _ctx)
    {
        if (_ctx.performed)
            maximapCoords = _ctx.ReadValue<Vector2>();

        Debug.LogError(maximapCoords);

        if (_ctx.canceled)
            maximapCoords = Vector2.zero;
    }
    public void OnZoom(CallbackContext _ctx)
    {
        maxiMapZoom = _ctx.ReadValue<Vector2>().y;
    }
    #endregion

    private void OpenMiniMap()
    {
        // Activate UI MiniMap Image
        m_MiniMapImage.gameObject.SetActive(true);
        minimapOpenFlag = true;
        // set normal minimapSize
        cam.orthographicSize = m_NormalMiniMapSize;
    }
    private void CloseMiniMap()
    {
        // Deactivate UI MiniMap Image
        m_MiniMapImage.gameObject.SetActive(false);
        minimapOpenFlag = false;
    }

    private void OpenMaxiMap()
    {
        // Change Camera Background Opacity to opague
        cam.backgroundColor = new Color(0, 0, 0, 1);
        // Activate MaxiMap
        m_MaxiMapImage.gameObject.SetActive(true);
        maximapOpenFlag = true;
        // Enable MaxiMap InputSystem controls
        GameManager.Instance.PlayerInput.currentActionMap = GameManager.Instance.PlayerInput.actions.FindActionMap("MaxiMap");
        // Set MaxiMap position to current zone
        cam.transform.position = ZoneManager.Instance.CurrentActiveZone.transform.position;
        // set normal maximap size
        cam.orthographicSize = m_NormalMaxiMapSize;
    }
    private void UpdateMaxiMapCoords()
    {
        // Update all zones
        UpdateZones();
        // Adjust Camera transform to Vector2
        cam.transform.position += (Vector3)maximapCoords.normalized;
        // Keep camera in bounds
        float x = cam.transform.position.x;
        float y = cam.transform.position.y;

        if (cam.transform.position.x > mapBoundsMax.x)
            x = mapBoundsMax.x;
        else if (cam.transform.position.x < mapBoundsMin.x)
            x = mapBoundsMin.x;
        if (cam.transform.position.y > mapBoundsMax.y)
            y = mapBoundsMax.y;
        else if (cam.transform.position.y < mapBoundsMin.y)
            y = mapBoundsMin.y;

        cam.transform.position = new Vector3(x, y, -10);
        // update maximap size and keep in bounds
        cam.orthographicSize += maxiMapZoom;

        if (cam.orthographicSize <= m_MinimumZoom)
            cam.orthographicSize = m_MinimumZoom;
        if(cam.orthographicSize >= m_MaximumZoom)
            cam.orthographicSize = m_MaximumZoom;
    }
    private void CloseMaxiMap()
    {
        // Change Camera Background Opacity to transparent
        cam.backgroundColor = new Color(0, 0, 0, 0);
        // Deactivate MaxiMap
        m_MaxiMapImage.gameObject.SetActive(false);
        maximapOpenFlag = false;
        // Enable Player Movement
        GameManager.Instance.PlayerInput.currentActionMap = GameManager.Instance.PlayerInput.actions.FindActionMap("Player");
        // Reset Map Coords
        cam.transform.position = new Vector3(0, 0, -10);
    }

    private void UpdateZones()
    {
        for (int i = 0; i < ZoneManager.Instance.Zones.Length; i++)
        {
            if (ZoneManager.Instance.Zones[i].WasVisited)
                ZoneManager.Instance.Zones[i].MapVisual.color = new Color(m_VisitedColor.r, m_VisitedColor.g, m_VisitedColor.b, m_Transparency);
            else
                ZoneManager.Instance.Zones[i].MapVisual.color = new Color(m_NormalColor.r, m_NormalColor.g, m_NormalColor.b, m_Transparency);
        }
        ZoneManager.Instance.CurrentActiveZone.MapVisual.color = new Color(m_CurrentZoneColor.r, m_CurrentZoneColor.g, m_CurrentZoneColor.b, m_Transparency);
    }
    private void CalculateBounds()
    {
        for (int i = 0; i < ZoneManager.Instance.Zones.Length; i++)
        {
            if (mapBoundsMin.x > ZoneManager.Instance.Zones[i].transform.position.x)
                mapBoundsMin.x = ZoneManager.Instance.Zones[i].transform.position.x;

            if (mapBoundsMin.y > ZoneManager.Instance.Zones[i].transform.position.y)
                mapBoundsMin.y = ZoneManager.Instance.Zones[i].transform.position.y;

            if (mapBoundsMax.x < ZoneManager.Instance.Zones[i].transform.position.x)
                mapBoundsMax.x = ZoneManager.Instance.Zones[i].transform.position.x;

            if (mapBoundsMax.y < ZoneManager.Instance.Zones[i].transform.position.y)
                mapBoundsMax.y = ZoneManager.Instance.Zones[i].transform.position.y;
        }
    }

    private void OnDestroy()
    {
        Terminate();
    }
}
