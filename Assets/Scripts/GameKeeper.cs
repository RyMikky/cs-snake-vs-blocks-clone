using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameKeeper : MonoBehaviour
{

    public GameObject _gameLevelPrefab;
    private GameObject _gameLevel;
    public GameObject _gameSnakePrefab;
    private GameObject _gameSnake;
    public GameObject _gameMenu;

    public Camera _gameCamera;
    public GameObject _gameSound;

    private GameUISystem _gameUISystem;
    private GameSoundSystem _gameSoundSystem;
    private GameConstantsKeeper _gameConstantsKeeper;
    private GameCameraSystem _gameCameraSystem;

    private void Awake()
    {
        _gameCameraSystem = GetComponent<GameCameraSystem>();
        _gameConstantsKeeper = GetComponent<GameConstantsKeeper>();
        _gameUISystem = _gameMenu.GetComponent<GameUISystem>();
        _gameSoundSystem = _gameSound.GetComponent<GameSoundSystem>();

        // ���������� �������������� ������������ ������� ��� ������, ������� ����� ����� ���������� ���������
        _gameLevel = Instantiate(_gameLevelPrefab, transform) as GameObject;

        // ���������� �������� ���� � ������� �� ���� ����������
        _gameUISystem.ActivateMenuScreen();
    }

    

    // ������������ ������� ������ ��������� � ����� �������
    void ConstructNewEasyLevel()
    {
        _gameSnake = Instantiate(_gameSnakePrefab, transform) as GameObject;
        ConstructEasyLevel();
    }

    // ������������ ������� ������ ��������� �� ������ �������
    void ConstructEasyLevel()
    {

    }



}