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
    [SerializeField] private PlayerInput m_Input;
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

    private void Start()
    {
        cam = GetComponent<Camera>();

        CalculateBounds();
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
        m_Input.currentActionMap = m_Input.actions.FindActionMap("MaxiMap");
        // Set MaxiMap position to current zone
        for (int i = 0; i < GameManager.Instance.Zones.Length; i++)
        {
            if (GameManager.Instance.Zones[i].IsActive)
                cam.transform.position = GameManager.Instance.Zones[i].transform.position;
        }
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
        m_Input.currentActionMap = m_Input.actions.FindActionMap("Player");
        // Reset Map Coords
        cam.transform.position = new Vector3(0, 0, -10);
    }

    private void UpdateZones()
    {
        for (int i = 0; i < GameManager.Instance.Zones.Length; i++)
        {
            if (GameManager.Instance.Zones[i].WasVisited)
                GameManager.Instance.Zones[i].MapVisual.color = new Color(m_VisitedColor.r, m_VisitedColor.g, m_VisitedColor.b, m_Transparency);
            else
                GameManager.Instance.Zones[i].MapVisual.color = new Color(m_NormalColor.r, m_NormalColor.g, m_NormalColor.b, m_Transparency);

            if (GameManager.Instance.Zones[i].IsActive)
                GameManager.Instance.Zones[i].MapVisual.color = new Color(m_CurrentZoneColor.r, m_CurrentZoneColor.g, m_CurrentZoneColor.b, m_Transparency);
        }
    }
    private void CalculateBounds()
    {
        for (int i = 0; i < GameManager.Instance.Zones.Length; i++)
        {
            if (mapBoundsMin.x > GameManager.Instance.Zones[i].transform.position.x)
                mapBoundsMin.x = GameManager.Instance.Zones[i].transform.position.x;

            if (mapBoundsMin.y > GameManager.Instance.Zones[i].transform.position.y)
                mapBoundsMin.y = GameManager.Instance.Zones[i].transform.position.y;

            if (mapBoundsMax.x < GameManager.Instance.Zones[i].transform.position.x)
                mapBoundsMax.x = GameManager.Instance.Zones[i].transform.position.x;

            if (mapBoundsMax.y < GameManager.Instance.Zones[i].transform.position.y)
                mapBoundsMax.y = GameManager.Instance.Zones[i].transform.position.y;
        }
    }
}
