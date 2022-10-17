using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemyData
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

    public void Spawn(List<GameObject> _enemyList)
    {
        AIController e = GameObject.Instantiate(EnemyPrefab);
        e.EnemyData = this;
        e.transform.position = SpawnPoint;
        _enemyList.Add(e.gameObject);
    }
}
[System.Serializable]
public struct SpawnAbles
{
    public EnemyData[] Enemies;

    public void SpawnAll(List<GameObject> _enemyList)
    {
        for (int i = 0; i < Enemies.Length; i++)
        {
            Enemies[i].Spawn(_enemyList);
        }
    }
    public void DestroyAll(List<GameObject> _enemyList)
    {
        for (int i = 0; i < _enemyList.Count; i++)
        {
            if (_enemyList[i] == null) continue;

            GameObject.Destroy(_enemyList[i]);
        }
    }
}
