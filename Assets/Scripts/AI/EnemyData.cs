using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemyData
{
    public AIController EnemyPrefab;

    public Vector2Int SpawnPoint;

    public Vector2Int[] Waypoints;

    public void Spawn(List<GameObject> _enemyList)
    {
        AIController e = GameObject.Instantiate(EnemyPrefab);
        e.EnemyData = this;
        e.transform.position = (Vector2)SpawnPoint;
        _enemyList.Add(e.gameObject);
    }
    public void SpawnAt(List<GameObject> _enemyList, Vector2Int _spawnPoint)
    {
        AIController e = GameObject.Instantiate(EnemyPrefab);
        e.EnemyData = this;
        e.EnemyData.SpawnPoint = _spawnPoint;
        e.transform.position = (Vector2)_spawnPoint;
        _enemyList.Add(e.gameObject);
    }
    public void SpawnDrops(Vector2 _position)
    {
        for (int i = 0; i < EnemyPrefab.Drops.Length; i++)
            EnemyPrefab.Drops[i].Drop(_position);
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
