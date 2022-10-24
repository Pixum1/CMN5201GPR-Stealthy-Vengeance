using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemyWaves
{
    public EnemyData[] EnemyTypes;
    public int AmountOfEnemies;
    public float TimeBetweenEnemySpawns;
    public bool IsFinished;

    public IEnumerator StartWave(List<GameObject> _enemyList, Vector2[] _locations)
    {
        for (int i = 0; i < AmountOfEnemies; i++)
        {
            int type = Random.Range(0, EnemyTypes.Length);
            int location = Random.Range(0, _locations.Length);

            EnemyTypes[type].SpawnAt(_enemyList, _locations[location]);
            yield return new WaitForSeconds(TimeBetweenEnemySpawns);
        }
        IsFinished = true;
    }
}
