using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraZone : MonoBehaviour
{
    public bool WasVisited = false;
    private bool isActive;
    public bool IsActive
    {
        get { return isActive; }
        set
        {
            if (value == isActive) return;

            isActive = value;

            if (isActive)
            {
                m_SpawnAbles.SpawnAll(enemiesInRoom);
                WasVisited = true;
            }
            else
            {
                m_SpawnAbles.DestroyAll(enemiesInRoom);
            }
        }
    }
    [SerializeField] public LayerMask playerLayer;
    [HideInInspector] public Collider col;
    [SerializeField] public float cameraOrthographicSize = 11;

    private enum ERoomType
    {
        normal,
        boss,
    }
    [SerializeField] private ERoomType m_RoomType;

    [SerializeField] private SpawnAbles m_SpawnAbles;
    private List<GameObject> enemiesInRoom = new List<GameObject>();


    private void Awake()
    {
        col = GetComponent<Collider>();
        CheckForPlayer();
    }

    private void Update()
    {
        CheckForPlayer();
    }
    /// <summary>
    /// Checks if the player is within the bounds of the zone.
    /// </summary>
    private void CheckForPlayer()
    {
        Collider2D[] cols = Physics2D.OverlapBoxAll(transform.position, transform.localScale, 0f, playerLayer);

        if (cols.Length != 0)
            IsActive = true;

        else
            IsActive = false;
    }


    private void OnDrawGizmos()
    {
        #region Zone Bounds
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, transform.localScale); //visualize zone bounds

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector2(cameraOrthographicSize * 2 * Camera.main.aspect, cameraOrthographicSize * 2));
        #endregion

        #region Spawnpoints
        if (m_SpawnAbles.Enemies != null)
        {
            for (int i = 0; i < m_SpawnAbles.Enemies.Length; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(m_SpawnAbles.Enemies[i].SpawnPoint, Vector2.one);

                for (int k = 0; k < m_SpawnAbles.Enemies[i].Waypoints.Length; k++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(m_SpawnAbles.Enemies[i].Waypoints[k], Vector2.one);
                }
            }
        }
        #endregion
    }
}
