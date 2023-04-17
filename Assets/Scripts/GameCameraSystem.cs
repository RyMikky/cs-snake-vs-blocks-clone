using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraSystem : MonoBehaviour
{ 
    private GameKeeper _mainGameKeeper;      // базовая система управления игрой и игровыми состояниями
    public GameCameraSystem SetGameKeeper(GameKeeper keeper) { _mainGameKeeper = keeper; return this; }

    public Camera _mainCamera;
    public float _currentScroll;

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
