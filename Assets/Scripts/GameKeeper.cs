using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private GameLevelSystem _gameLevelSystem;
    private SnakeGameUnitSystem _snakeGameUnitSystem;
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

        // активирует автоматический бекграундный уровень без змейки, который фоном будет бесконечно двигаться
        _gameLevel = Instantiate(_gameLevelPrefab, transform) as GameObject;
        _gameLevelSystem = _gameLevel.GetComponent<GameLevelSystem>();

        _gameLevelSystem.SetGameKeeper(this).ConstructNewLevelSession(
                _gameConstantsKeeper.GetLevelConfiguration(GameConstantsKeeper.GameDifficulty.demo));

        Transform snakeStartPosition = transform;
        snakeStartPosition.position = new Vector3(0, 2.75f, 10);

        // активирует змейку с выключенным триггером и разрешенным управлением с клавиатуры
        _gameSnake = Instantiate(_gameSnakePrefab, transform) as GameObject;
        _gameSnake.transform.localPosition = new Vector3(0, 2.75f, 10);
        _snakeGameUnitSystem = _gameSnake.GetComponent<SnakeGameUnitSystem>();

        _snakeGameUnitSystem.SetGameKeeper(this).SetGameUISystem(_gameUISystem).ConstructNewSnake(
                _gameConstantsKeeper.GetLevelConfiguration(GameConstantsKeeper.GameDifficulty.demo),
                true, true, false, true, false, true);

        // активирует основное меню и передаёт на него управление
        
        _gameUISystem.ActivateMenuScreen();
    }

    // -------------------------------------- блок конструкторов уровня ---------------------------------------

    // конструирует уровень легкой сложности с новой змейкой
    public void ConstructNewEasyLevel()
    {
        _gameSnake = Instantiate(_gameSnakePrefab, transform) as GameObject;
        ConstructEasyLevel();
    }

    // конструирует уровень легкой сложности со старой змейкой
    public void ConstructEasyLevel()
    {
        _gameLevel.GetComponent<GameLevelSystem>()
            .ConstructNewLevelSession(
                _gameConstantsKeeper.GetLevelConfiguration(GameConstantsKeeper.GameDifficulty.easy));

        _snakeGameUnitSystem.ConstructNewSnake(
                _gameConstantsKeeper.GetLevelConfiguration(GameConstantsKeeper.GameDifficulty.easy),
                false, false, true, true, false, true);
    }

    // конструирует уровень средней сложности со старой змейкой
    public void ConstructNormalLevel()
    {
        _gameLevel.GetComponent<GameLevelSystem>()
            .ConstructNewLevelSession(
                _gameConstantsKeeper.GetLevelConfiguration(GameConstantsKeeper.GameDifficulty.normal));

        _snakeGameUnitSystem.ConstructNewSnake(
                _gameConstantsKeeper.GetLevelConfiguration(GameConstantsKeeper.GameDifficulty.normal),
                false, false, true, true, false, true);
    }

    // конструирует уровень высокой сложности со старой змейкой
    public void ConstructHardLevel()
    {
        _gameLevel.GetComponent<GameLevelSystem>()
            .ConstructNewLevelSession(
                _gameConstantsKeeper.GetLevelConfiguration(GameConstantsKeeper.GameDifficulty.hard));

        _snakeGameUnitSystem.ConstructNewSnake(
                _gameConstantsKeeper.GetLevelConfiguration(GameConstantsKeeper.GameDifficulty.hard),
                false, false, true, true, false, true);
    }

    // конструирует уровень оч.высокой сложности со старой змейкой
    public void ConstructInsaneLevel()
    {
        _gameLevel.GetComponent<GameLevelSystem>()
            .ConstructNewLevelSession(
                _gameConstantsKeeper.GetLevelConfiguration(GameConstantsKeeper.GameDifficulty.insane));

        _snakeGameUnitSystem.ConstructNewSnake(
                _gameConstantsKeeper.GetLevelConfiguration(GameConstantsKeeper.GameDifficulty.insane),
                false, false, true, true, false, true);
    }

    // -------------------------------------- блок игровых состояний ------------------------------------------

    // функция вызывающая состояние проигрыша
    public void GameOver(int score)
    {
        
        _gameUISystem.ActivateLoserScreen();                                     // активируем экран поражения
    }

    public void LevelComplette(int score, int extraLifes)
    {
        _gameUISystem.ActivateWinnerScreen();                                    // активируем экран победы
    }
}
