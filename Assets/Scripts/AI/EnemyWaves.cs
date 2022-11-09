using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemyWaves
{
    public EnemyData[] EnemyTypes;
    public Vector2Int[] SpawnPoints;
    public int AmountOfEnemies;
    public float TimeBetweenEnemySpawns;

    public IEnumerator StartWave(List<GameObject> _enemyList)
    {
        for (int i = 0; i < AmountOfEnemies; i++)
        {
            int type = Random.Range(0, EnemyTypes.Length);
            int location = Random.Range(0, SpawnPoints.Length);

            EnemyTypes[type].SpawnAt(_enemyList, SpawnPoints[location]);
            yield return new WaitForSeconds(TimeBetweenEnemySpawns);
        }
        Debug.LogWarning("Wave is Finished");
    }
}
