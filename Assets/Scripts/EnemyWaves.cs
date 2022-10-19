using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemyWave
{
    public EventEnemyData[] EnemyTypes;
    public Vector2[] Spawnpoints;
    public int AmountOfEnemies;
    public float TimeBetweenEnemySpawns;

    public IEnumerator StartWave()
    {
        for (int i = 0; i < AmountOfEnemies; i++)
        {
            int type = Random.Range(0, EnemyTypes.Length);
            int location = Random.Range(0, Spawnpoints.Length);

            EnemyTypes[type].Spawn(Spawnpoints[location]);
            yield return new WaitForSeconds(TimeBetweenEnemySpawns);
        }
    }

}
[System.Serializable]
public struct EventEnemyData
{
    public AIController EnemyPrefab;

    public float Speed;

    [Header("Combat")]
    public Projectile ProjectilePrefab;
    public float ShootForce;
    public float ShootCooldown;
    public float AttackRange;

    public float VisionRange;

    public void Spawn(Vector2 _position)
    {
        AIController e = GameObject.Instantiate(EnemyPrefab);
        e.EnemyData = new EnemyData(
            EnemyPrefab, 
            _position, 
            0, 
            Speed, 
            null, 
            ProjectilePrefab,
            ShootForce,
            ShootCooldown,
            AttackRange,
            VisionRange,
            null);

        e.transform.position = _position;
    }
}
