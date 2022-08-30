using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField] private List<InstancedGroupSpawner> _spawners;
    [SerializeField] private Material _GPUInstancedMaterial;
    [SerializeField] private int _maxSizeArea;

    private SortedList<(string, string), float> _combinationForceTable;
    private List<string> _names;
    private List<ObjectData> _particles;

    void Start()
    {
        _names = new List<string>();
        _combinationForceTable = new SortedList<(string, string), float>();
        _particles = new List<ObjectData>();

        InitializeSpawners();
        CreateCombinationTable();

        foreach (InstancedGroupSpawner spawner in _spawners)
        {
            _particles.AddRange(spawner.GetObjects());
        }
    }

    void Update()
    {
        foreach (InstancedGroupSpawner spawner in _spawners)
        {
            spawner.UpdateBatches();
        }

        AssignForces(SolveForces());
    }

    private void InitializeSpawners()
    {
        foreach (InstancedGroupSpawner spawner in _spawners)
        {
            spawner.maxPosition = new Vector2(_maxSizeArea, _maxSizeArea);
            spawner.InitializeSpawner(_GPUInstancedMaterial);
            _names.Add(spawner.colorName);
        }
    }

    public void CreateCombinationTable()
    {
        _combinationForceTable.Clear();
        for (int i = 0; i < _names.Count; i++)
        {
            for (int j = 0; j < _names.Count; j++)
            {
                if (_combinationForceTable.ContainsKey((_names[i], _names[j]))) continue;
                _combinationForceTable.Add((_names[i], _names[j]), Random.Range(-5f, 5f));
            }
        }
    }

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
                    float F = _combinationForceTable[(a.color, b.color)] * 1.0f / d;
                    acceleration.x += (F * dx);
                    acceleration.y += (F * dy);
                }
            }

            a.velocity.x = (a.velocity.x + acceleration.x) * 0.5f;
            a.velocity.y = (a.velocity.y + acceleration.y) * 0.5f;

            Vector2 newPosition = new Vector2(a.position.x + a.velocity.x * Time.deltaTime, a.position.y + a.velocity.y * Time.deltaTime);
            if (newPosition.x <= -_maxSizeArea - 10 || newPosition.x >= _maxSizeArea + 10) newPosition *= -1;
            if (newPosition.y <= -_maxSizeArea - 10 || newPosition.y >= _maxSizeArea + 10) newPosition *= -1;

            newPositions.Add(newPosition);
        }

        return newPositions;
    }

    private void AssignForces(List<Vector2> pForces)
    {
        for (int i = 0; i < pForces.Count; i++)
        {
            _particles[i].position = pForces[i];
        }
    }
}
