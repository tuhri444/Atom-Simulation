using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField] private List<InstancedGroupSpawner> _spawners;
    [SerializeField] private Material _GPUInstancedMaterial;
    [SerializeField] private int _maxSizeArea;

    public Vector2 _minMaxAttractionForce;

    private SortedList<(int, int), float> _combinationForceTable;
    private List<ObjectData> _particles;
    private List<string> _names;

    void Start()
    {
        _combinationForceTable = new SortedList<(int, int), float>();
        _particles = new List<ObjectData>();
        _names = new List<string>();

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
            _names.Add(spawner.spawnerName);
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
                _combinationForceTable.Add((i, j), Random.Range(_minMaxAttractionForce.x, _minMaxAttractionForce.y));
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
    /// Allows you to change individual combo values in the global list and will then adjust the local combo tables.
    /// </summary>
    /// <param name="pKey">The combination you are looking to change.</param>
    /// <param name="pValue">The value you want to assign to the combo.</param>
    public void AdjustGlobalComboTable((int,int) pKey, float pValue)
    {
        _combinationForceTable[pKey] = pValue;
        AdjustLocalComboTables();
    }

    /// <summary>
    /// Gets the list containing all combinations of a certain type and their attraction force.
    /// </summary>
    /// <returns>SortedList with the key being the particle combo and the value being a float that holds the attraction force.</returns>
    public SortedList<(int, int), float> GetGlobalComboTableOfType(int pType)
    {
        SortedList<(int, int), float> output = new SortedList<(int, int), float>();
        foreach((int, int) key in _combinationForceTable.Keys)
        {
            if(key.Item1 == pType)
            {
                output.Add(key, _combinationForceTable[key]);
            }
        }
        return output;
    }

    /// <summary>
    /// Gets a list containing all the names of the different types of particles in the order of spawner IDs.
    /// </summary>
    /// <returns>List with all the names of each spawner.</returns>
    public List<string> GetParticleNames()
    {
        return _names;
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
