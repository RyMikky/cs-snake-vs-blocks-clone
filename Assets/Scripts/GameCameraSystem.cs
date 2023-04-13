using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraSystem : MonoBehaviour
{

    public Camera _mainCamera;
    public float _currentScroll;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CameraFieldsUpdate();
    }

    void CameraFieldsUpdate()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        _mainCamera.fieldOfView -= (scroll * 10);
    }
}
