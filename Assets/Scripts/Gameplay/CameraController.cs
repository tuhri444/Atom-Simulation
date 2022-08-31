using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool canDrag = true;

    [SerializeField] private float _zoomSpeed = 5f;
    [SerializeField] private float _zoomMagnitude = 0.01f;
    [SerializeField] private float _MouseTravelSpeed = 5f;
    [SerializeField] private float _MouseTravelMagnitude = 0.01f;
    [SerializeField] private float _preferredZPosition = -10f;

    private Camera _mainCamera;
    private Vector3 _mouseDownStartPosition;
    private Vector3 _oldCameraPosition;
    private Vector3 _newCameraPosition;

    private float _newZoomSize;
    private bool _hasPressed;

    void Start()
    {
        _mainCamera = GetComponent<Camera>();
        _newZoomSize = _mainCamera.orthographicSize;
        _newCameraPosition = _mainCamera.transform.position;
    }
    void Update()
    {
        Zoom();
        if (canDrag)
        {
            DragMove();
        }
    }

    private void Zoom()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            _newZoomSize = Mathf.Clamp(_mainCamera.orthographicSize + -Input.mouseScrollDelta.y * _zoomMagnitude * _mainCamera.orthographicSize, 0.1f, Mathf.Infinity);
        }
        _mainCamera.orthographicSize = Mathf.Lerp(_mainCamera.orthographicSize, _newZoomSize, _zoomSpeed * Time.deltaTime);
    }

    private void DragMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _mouseDownStartPosition = Input.mousePosition;
            _oldCameraPosition = _mainCamera.transform.position;
            _newCameraPosition = _mainCamera.transform.position;
            _hasPressed = true;
        }
        else if (Input.GetMouseButton(0) && _hasPressed)
        {
            Vector2 mouseTravel = -(Input.mousePosition - _mouseDownStartPosition);
            float distanceMouseTravel = mouseTravel.magnitude;
            Vector3 directionMouseTravel = mouseTravel.normalized;
            _newCameraPosition = _oldCameraPosition + (_MouseTravelMagnitude * distanceMouseTravel * directionMouseTravel * _mainCamera.orthographicSize);
            _newCameraPosition.z = _preferredZPosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _hasPressed = false;
        }
        _mainCamera.transform.position = Vector3.Lerp(_mainCamera.transform.position, _newCameraPosition, _MouseTravelSpeed * Time.deltaTime);
    }
}
