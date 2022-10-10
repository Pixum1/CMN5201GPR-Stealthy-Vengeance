using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraZone : MonoBehaviour
{
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
                SpawnAllEnemies();
            }
            else
            {
                DestroyAllEnemies();
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

    [SerializeField] private EnemyData[] m_Enemy;
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

    public void SpawnAllEnemies()
    {
        for (int i = 0; i < m_Enemy.Length; i++)
            SpawnEnemy(m_Enemy[i]);
    }
    public void DestroyAllEnemies()
    {
        for (int i = 0; i < enemiesInRoom.Count; i++)
        {
            if (enemiesInRoom[i] == null) continue;

            Destroy(enemiesInRoom[i]);
        }
    }
    private void SpawnEnemy(EnemyData _enemy)
    {
        AIController e = Instantiate(_enemy.EnemyPrefab);
        e.EnemyData = _enemy;
        e.transform.position = _enemy.SpawnPoint;
        enemiesInRoom.Add(e.gameObject);
    }

    private void OnDisable()
    {
        FindObjectOfType<ZoneManager>().RemoveZoneFromList(this);
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
        if (m_Enemy != null)
        {
            for (int i = 0; i < m_Enemy.Length; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(m_Enemy[i].SpawnPoint, Vector2.one);

                for (int k = 0; k < m_Enemy[i].Waypoints.Length; k++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(m_Enemy[i].Waypoints[k], Vector2.one);
                }
            }
        }
        #endregion
    }
}
