using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ObjectData
{
    public Vector2 position;
    public Vector2 scale;
    public Quaternion rotation;

    public ObjectData(Vector2 pPosition, Vector2 pScale, Quaternion pRotation)
    {
        position = pPosition;
        scale = pScale;
        rotation = pRotation;
    }

    public Matrix4x4 matrix
    {
        get
        {
            return Matrix4x4.TRS(position, rotation, scale);
        }
    }
}

public class Instanced_Spawning : MonoBehaviour
{
    [SerializeField] private int _instances;
    [SerializeField] private Vector2 _maxPosition;
    [SerializeField] private Mesh _objectMesh;
    [SerializeField] private Material _objectMaterial;

    private List<List<ObjectData>> batches = new List<List<ObjectData>>();

    void Start()
    {
        int batchIndexNumber = 0;
        List<ObjectData> currentBatch = new List<ObjectData>();
        for (int i = 0; i < _instances; i++)
        {
            AddObject(currentBatch, i);
            batchIndexNumber++;
            if (batchIndexNumber < 500) continue;

            batches.Add(currentBatch);
            currentBatch = BuildNewBatch();
            batchIndexNumber = 0;
        }
    }

    void Update()
    {
        RenderBatches();
    }

    private void AddObject(List<ObjectData> currentBatch, int i)
    {
        Vector2 position = new Vector2(UnityEngine.Random.Range(-_maxPosition.x, _maxPosition.x), UnityEngine.Random.Range(-_maxPosition.y, _maxPosition.y));
        currentBatch.Add(new ObjectData(position, new Vector2(2, 2), Quaternion.identity));
    }

    private List<ObjectData> BuildNewBatch()
    {
        return new List<ObjectData>();
    }

    private void RenderBatches()
    {
        foreach(var batch in batches)
        {
            Graphics.DrawMeshInstanced(_objectMesh, 0, _objectMaterial, batch.Select((a) => a.matrix).ToList());
        }
    }
}
