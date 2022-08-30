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

public abstract class InstancedGroupSpawner : ScriptableObject
{
    public int instances;
    public Vector2 maxPosition;
    public Mesh objectMesh;
    public Color objectColor;

    protected Material _objectMaterial;
    protected List<List<ObjectData>> _batches = new List<List<ObjectData>>();

    public void InitializeSpawner(Material _baseMaterial)
    {
        _objectMaterial = new Material(_baseMaterial);
        _objectMaterial.color = objectColor;

        InitiliazeBatches();
    }
    public virtual void UpdateBatches()
    {
        RenderBatches();
    }

    private void InitiliazeBatches()
    {
        int batchIndexNumber = 0;
        List<ObjectData> currentBatch = new List<ObjectData>();
        for (int i = 0; i < instances; i++)
        {
            AddObject(currentBatch, i);
            batchIndexNumber++;
            if (batchIndexNumber < 500) continue;

            _batches.Add(currentBatch);
            currentBatch = BuildNewBatch();
            batchIndexNumber = 0;
        }
    }

    private void AddObject(List<ObjectData> currentBatch, int i)
    {
        Vector2 position = new Vector2(UnityEngine.Random.Range(-maxPosition.x, maxPosition.x), UnityEngine.Random.Range(-maxPosition.y, maxPosition.y));
        currentBatch.Add(new ObjectData(position, new Vector2(2, 2), Quaternion.identity));
    }

    private List<ObjectData> BuildNewBatch()
    {
        return new List<ObjectData>();
    }

    private void RenderBatches()
    {
        foreach(var batch in _batches)
        {
            Graphics.DrawMeshInstanced(objectMesh, 0, _objectMaterial, batch.Select((a) => a.matrix).ToList());
        }
    }
}
