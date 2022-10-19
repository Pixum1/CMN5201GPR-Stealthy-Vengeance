using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemyData
{
    public AIController EnemyPrefab;

    public Vector2 SpawnPoint;

    public float IdleTime;
    public float Speed;
    public Vector2[] Waypoints;

    [Header("Combat")]
    public Projectile ProjectilePrefab;
    public float ShootForce;
    public float ShootCooldown;
    public float AttackRange;

    public float VisionRange;

    public Drops[] Drops;

    public EnemyData(AIController _prefab, Vector2 _spawnpoint, float _idleTime, float _speed, Vector2[] _waypoints, Projectile _projectile, float _shootForce, float _shootCD, float _attRange, float _visionRange, Drops[] _drops)
    {
        EnemyPrefab = _prefab;
        SpawnPoint = _spawnpoint;
        IdleTime = _idleTime;
        Speed = _speed;
        Waypoints = _waypoints;
        ProjectilePrefab = _projectile;
        ShootForce = _shootForce;
        ShootCooldown = _shootCD;
        AttackRange = _attRange;
        VisionRange = _visionRange;
        Drops = _drops;
    }

    public void Spawn(List<GameObject> _enemyList)
    {
        AIController e = GameObject.Instantiate(EnemyPrefab);
        e.EnemyData = this;
        e.transform.position = SpawnPoint;
        _enemyList.Add(e.gameObject);
    }
    public void SpawnDrops(Vector2 _position)
    {
        for (int i = 0; i < Drops.Length; i++)
            Drops[i].Drop(_position);
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
[System.Serializable]
public struct Drops
{
    public Item Item;
    [Range(0, 1)] public float DropChance;
    public bool Despawn;
    public float DespawnTime;
    public void Drop(Vector2 _position)
    {
        if (Random.Range(0f, 1f) <= DropChance)
        {
            Item i = GameObject.Instantiate(Item);
            i.transform.position = _position;
            i.Despawn = Despawn;
            i.despawnTime = DespawnTime;
        }
    }
}