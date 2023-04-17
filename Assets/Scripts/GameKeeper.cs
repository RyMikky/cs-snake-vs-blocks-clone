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

        // активирует автоматический бекграундный уровень, который фоном будет бесконечно двигаться
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

    
    private GameConstantsKeeper.GameDifficulty _currentDifficulty;      // поле для отслеживания текущего уровня сложности игры
    private int _lastGameScore;                                         // поле сохранения последнего количества очков
    private int _lastExtraLife;                                         // поле сохранения последнего количества экстра-жизней

    // -------------------------------------- блок конструкторов уровня ---------------------------------------

    // конструирует уровень легкой сложности с новой змейкой
    public void ConstructNewEasyLevel()
    {
        _lastGameScore = 0;
        _lastExtraLife = _gameConstantsKeeper
            .GetLevelConfiguration(GameConstantsKeeper.GameDifficulty.easy)._snakeExtraLife;

        ConstructEasyLevel(true, true);
    }
    // конструирует уровень легкой сложности с заданными параметрами змейки
    public void ConstructEasyLevel(bool reset_extra_life, bool reset_score)
    {
        _currentDifficulty = GameConstantsKeeper.GameDifficulty.easy;     // сохраняем сложность

        _gameLevel.GetComponent<GameLevelSystem>()
            .ConstructNewLevelSession(
                _gameConstantsKeeper.GetLevelConfiguration(_currentDifficulty));

        _snakeGameUnitSystem.SetGameUISystem(_gameUISystem).ConstructNewSnake(
                _gameConstantsKeeper.GetLevelConfiguration(_currentDifficulty),
                reset_extra_life, reset_score, true, true, false, true);

        _currentDifficulty = GameConstantsKeeper.GameDifficulty.easy;     // сохраняем сложность

        _gameUISystem.SetTextLevel("Легкий").ActivateGameMode();          // перевод UI в режим отображения игровых данных
    }


    // конструирует уровень cредней сложности с новой змейкой
    public void ConstructNewNormalLevel()
    {
        _lastGameScore = 0;
        _lastExtraLife = _gameConstantsKeeper
            .GetLevelConfiguration(GameConstantsKeeper.GameDifficulty.normal)._snakeExtraLife;

        ConstructNormalLevel(true, true);
    }
    // конструирует уровень средней сложности со старой змейкой
    public void ConstructNormalLevel(bool reset_extra_life, bool reset_score)
    {
        _currentDifficulty = GameConstantsKeeper.GameDifficulty.normal;     // сохраняем сложность

        _gameLevel.GetComponent<GameLevelSystem>()
            .ConstructNewLevelSession(
                _gameConstantsKeeper.GetLevelConfiguration(_currentDifficulty));

        _snakeGameUnitSystem.SetGameUISystem(_gameUISystem).ConstructNewSnake(
                _gameConstantsKeeper.GetLevelConfiguration(_currentDifficulty),
                reset_extra_life, reset_score, true, true, false, true);

        _gameUISystem.SetTextLevel("Cредний").ActivateGameMode();           // перевод UI в режим отображения игровых данных
    }


    // конструирует уровень высокой сложности с новой змейкой
    public void ConstructNewHardLevel()
    {
        _lastGameScore = 0;
        _lastExtraLife = _gameConstantsKeeper
            .GetLevelConfiguration(GameConstantsKeeper.GameDifficulty.hard)._snakeExtraLife;

        ConstructHardLevel(true, true);
    }
    // конструирует уровень высокой сложности со старой змейкой
    public void ConstructHardLevel(bool reset_extra_life, bool reset_score)
    {
        _currentDifficulty = GameConstantsKeeper.GameDifficulty.hard;     // сохраняем сложность

        _gameLevel.GetComponent<GameLevelSystem>()
            .ConstructNewLevelSession(
                _gameConstantsKeeper.GetLevelConfiguration(_currentDifficulty));

        _snakeGameUnitSystem.SetGameUISystem(_gameUISystem).ConstructNewSnake(
                _gameConstantsKeeper.GetLevelConfiguration(_currentDifficulty),
                reset_extra_life, reset_score, true, true, false, true);

        _gameUISystem.SetTextLevel("Cложный").ActivateGameMode();          // перевод UI в режим отображения игровых данных
    }


    // конструирует уровень оч.высокой сложности с новой змейкой
    public void ConstructNewInsaneLevel()
    {
        _lastGameScore = 0;
        _lastExtraLife = _gameConstantsKeeper
            .GetLevelConfiguration(GameConstantsKeeper.GameDifficulty.insane)._snakeExtraLife;

        ConstructInsaneLevel(true, true);
    }
    // конструирует уровень оч.высокой сложности с новой змейкой
    public void ConstructInsaneLevel(bool reset_extra_life, bool reset_score)
    {
        _currentDifficulty = GameConstantsKeeper.GameDifficulty.insane;     // сохраняем сложность

        _gameLevel.GetComponent<GameLevelSystem>()
            .ConstructNewLevelSession(
                _gameConstantsKeeper.GetLevelConfiguration(_currentDifficulty));

        _snakeGameUnitSystem.SetGameUISystem(_gameUISystem).ConstructNewSnake(
                _gameConstantsKeeper.GetLevelConfiguration(_currentDifficulty),
               reset_extra_life, reset_score, true, true, false, true);

        _gameUISystem.SetTextLevel("Адочек!").ActivateGameMode();            // перевод UI в режим отображения игровых данных
    }


    // -------------------------------------- блок игровых состояний ------------------------------------------

    // конструирует следующий уровень сложности при прохождении предыдущего
    public void PlayNextDifficulty()
    {
        switch (_currentDifficulty)
        {
            case GameConstantsKeeper.GameDifficulty.easy:
                ConstructNormalLevel(false, false);
                break;

            case GameConstantsKeeper.GameDifficulty.normal:
                ConstructHardLevel(false, false);
                break;

            case GameConstantsKeeper.GameDifficulty.hard:
                ConstructInsaneLevel(false, false);
                break;

            case GameConstantsKeeper.GameDifficulty.insane:

                // TODO сделать обработку случая когда пройдены все уровни

                break;
        }
    }
    // создаёт новый уровень с последними сохраненными параметрами змейки по очкам и экстра жизням
    public void RestartLastDifficulty()
    {
        switch (_currentDifficulty)
        {
            case GameConstantsKeeper.GameDifficulty.easy:
                ConstructEasyLevel(false, false);
                _snakeGameUnitSystem.SetSnakeExtraLife(_lastExtraLife).SetSnakeCurrentScore(_lastGameScore);
                break;

            case GameConstantsKeeper.GameDifficulty.normal:
                ConstructNormalLevel(false, false);
                _snakeGameUnitSystem.SetSnakeExtraLife(_lastExtraLife).SetSnakeCurrentScore(_lastGameScore);
                break;

            case GameConstantsKeeper.GameDifficulty.hard:
                ConstructHardLevel(false, false);
                _snakeGameUnitSystem.SetSnakeExtraLife(_lastExtraLife).SetSnakeCurrentScore(_lastGameScore);
                break;

            case GameConstantsKeeper.GameDifficulty.insane:
                ConstructInsaneLevel(false, false);
                _snakeGameUnitSystem.SetSnakeExtraLife(_lastExtraLife).SetSnakeCurrentScore(_lastGameScore);
                break;
        }
    }

    // функция вызывающая состояние проигрыша
    public void GameOver(int score)
    {
        _gameUISystem.ActivateLoserScreen();                                     // активируем экран поражения
    }
    // вызывает состояние победы
    public void LevelComplette(int score, int extraLifes)
    {
        _lastGameScore = score; _lastExtraLife = extraLifes;                     // сохраняет очки и жизни
        _gameUISystem.ActivateWinnerScreen();                                    // активируем экран победы
    }
}