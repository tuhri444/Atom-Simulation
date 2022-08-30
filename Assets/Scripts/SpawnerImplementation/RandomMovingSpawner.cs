using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomMovingSpawner", menuName = "Atoms/RandomMovingSpawner", order = 0)]
public class RandomMovingSpawner : InstancedGroupSpawner
{
    public override void UpdateBatches()
    {
        base.UpdateBatches();
        for (int i = 0; i < _batches.Count; i++)
        {
            for (int j = 0; j < _batches[i].Count; j++)
            {
                _batches[i][j].position.x += UnityEngine.Random.Range(-8, 8) * Time.deltaTime;
                _batches[i][j].position.y += UnityEngine.Random.Range(-8, 8) * Time.deltaTime;
            }
        }
    }
}
