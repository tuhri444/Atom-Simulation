using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _zoomSpeed;

    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = GetComponent<Camera>();
    }
    void Update()
    {
        if (Input.mouseScrollDelta.y == 0) return;
        _mainCamera.orthographicSize = Mathf.Clamp(_mainCamera.orthographicSize + Input.mouseScrollDelta.y * _zoomSpeed, 0.1f, Mathf.Infinity);
    }

}
