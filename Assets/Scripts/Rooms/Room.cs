using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Enemy
{
    public AIController EnemyPrefab;

    public Vector2 SpawnPoint;

    public float IdleTime;
    public float RoamSpeed;
    public Vector2[] Waypoints;

    [Header("Combat")]
    public Projectile ProjectilePrefab;
    public float ShootForce;
    public float ShootCooldown;
    public float AttackRange;

    public float VisionRange;
}

public class Room : MonoBehaviour
{
    [SerializeField] private Enemy[] m_Enemy;
    private List<GameObject> enemiesInRoom = new List<GameObject>();

    public void SpawnAllEnemies()
    {
        for (int i = 0; i < m_Enemy.Length; i++)
        {
            SpawnEnemy(m_Enemy[i]);
        }
    }
    public void DestroyAllEnemies()
    {
        for (int i = 0; i < enemiesInRoom.Count; i++)
        {
            if (enemiesInRoom[i] == null) continue;

            Destroy(enemiesInRoom[i]);
        }
    }

    private void SpawnEnemy(Enemy _enemy)
    {
        AIController e = Instantiate(_enemy.EnemyPrefab);
        e.EnemyData = _enemy;
        e.transform.position = _enemy.SpawnPoint;
        enemiesInRoom.Add(e.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < m_Enemy.Length; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(m_Enemy[i].SpawnPoint, Vector2.one);

            if (m_Enemy[i].Waypoints == null) break;

            for (int k = 0; k < m_Enemy[i].Waypoints.Length; k++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(m_Enemy[i].Waypoints[k], Vector2.one);
            }
        }
    }
}
