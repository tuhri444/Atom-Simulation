using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField] private List<InstancedGroupSpawner> _spawners;
    [SerializeField] private Material _GPUInstancedMaterial;
    [SerializeField] private int _maxSizeArea;

    private SortedList<(int, int), float> _combinationForceTable;
    private List<ObjectData> _particles;

    void Start()
    {
        _combinationForceTable = new SortedList<(int, int), float>();
        _particles = new List<ObjectData>();

        InitializeSpawners();

        foreach (InstancedGroupSpawner spawner in _spawners)
        {
            _particles.AddRange(spawner.GetObjects());
        }

        CreateRandomComboTable();
        AdjustLocalComboTables();
    }

    void Update()
    {
        foreach (InstancedGroupSpawner spawner in _spawners)
        {
            spawner.UpdateBatches();
        }
        AssignForces(SolveForces());
    }

    /// <summary>
    /// Initialize each spawner with the prefab material that contains the right parameters.
    /// Additionally they are passed their own ID and the amount of different elements there are. 
    /// This is to let the particles create a local combo table array with the right size.
    /// Lastly, the max spawning area is passed on to each spawner.
    /// </summary>
    private void InitializeSpawners()
    {
        int i = 0;
        foreach (InstancedGroupSpawner spawner in _spawners)
        {
            spawner.maxPosition = new Vector2(_maxSizeArea, _maxSizeArea);
            spawner.InitializeSpawner(_GPUInstancedMaterial, _spawners.Count, i);
            i++;
        }
    }

    /// <summary>
    /// Generates a random combo table, it will also automatically adjust the local tables.
    /// </summary>
    public void CreateRandomComboTable()
    {
        _combinationForceTable.Clear();
        for (int i = 0; i < _spawners.Count; i++)
        {
            for (int j = 0; j < _spawners.Count; j++)
            {
                if (_combinationForceTable.ContainsKey((i, j))) continue;
                _combinationForceTable.Add((i, j), Random.Range(-10f, 10f));
            }
        }
    }

    /// <summary>
    /// Changes the local element combo table saved in each particle based on the changes in the global combo table.
    /// </summary>
    public void AdjustLocalComboTables()
    {
        for (int i = 0; i < _particles.Count; i++)
        {
            for (int j = 0; j < _spawners.Count; j++)
            {
                _particles[i].localComboTable[j] = _combinationForceTable[(_particles[i].id, j)];
            }
        }
    }


    /// <summary>
    /// Solves the physics calculations between all the particles.
    /// </summary>
    /// <returns>List of all the new positions of the particles.</returns>
    private List<Vector2> SolveForces()
    {
        List<Vector2> newPositions = new List<Vector2>();

        for (int i = 0; i < _particles.Count; i++)
        {
            Vector2 acceleration = new Vector2();
            ObjectData a = _particles[i];
            for (int j = 0; j < _particles.Count; j++)
            {
                if (_particles[i] == _particles[j]) continue;
                ObjectData b = _particles[j];

                float dx = a.position.x - b.position.x;
                float dy = a.position.y - b.position.y;

                float d = Mathf.Sqrt(dx * dx + dy * dy);
                if (d > 0 && d < 80)
                {
                    float F = a.localComboTable[b.id] * 1.0f / d;
                    acceleration.x += (F * dx);
                    acceleration.y += (F * dy);
                }
            }

            a.velocity.x = (a.velocity.x + acceleration.x) * Time.deltaTime;
            a.velocity.y = (a.velocity.y + acceleration.y) * Time.deltaTime;

            Vector2 newPosition = new Vector2(a.position.x + a.velocity.x, a.position.y + a.velocity.y);
            if (newPosition.x <= -_maxSizeArea || newPosition.x >= _maxSizeArea) a.velocity.x *= -1;
            if (newPosition.y <= -_maxSizeArea || newPosition.y >= _maxSizeArea) a.velocity.y *= -1;

            newPositions.Add(newPosition);
        }

        return newPositions;
    }

    /// <summary>
    /// Simply assigns all the new positions to each particles.
    /// </summary>
    /// <param name="pForces">A list of new positions for each particle.</param>
    private void AssignForces(List<Vector2> pForces)
    {
        for (int i = 0; i < pForces.Count; i++)
        {
            _particles[i].position = pForces[i];
        }
    }
}
