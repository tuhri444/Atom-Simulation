using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseCameraMovement : MonoBehaviour
{
    private CameraController _cameraController;
    void Start()
    {
        _cameraController = FindObjectOfType<CameraController>();
    }

    public void OnMouseEnter()
    {
        if (_cameraController.canDrag == true) _cameraController.canDrag = false;
    }

    public void OnMouseExit()
    {
        if (_cameraController.canDrag == false) _cameraController.canDrag = true;
    }
}
