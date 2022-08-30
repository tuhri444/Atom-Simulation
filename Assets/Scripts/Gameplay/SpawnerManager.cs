using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField] private List<InstancedGroupSpawner> _spawners;
    [SerializeField] private Material _GPUInstancedMaterial;
    void Start()
    {
        foreach(InstancedGroupSpawner spawner in _spawners)
        {
            spawner.InitializeSpawner(_GPUInstancedMaterial);
        }
    }

    void Update()
    {
        foreach (InstancedGroupSpawner spawner in _spawners)
        {
            spawner.UpdateBatches();
        }
    }
}
