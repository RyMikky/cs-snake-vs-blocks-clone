using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelSystem : MonoBehaviour
{
    private GameKeeper _mainGameKeeper;     // базовая система управления игрой и игровыми состояниями
    public GameLevelSystem SetGameKeeper(GameKeeper keeper) { _mainGameKeeper = keeper; return this; }


    public GameObject _snakeUnitPrefab;                            // префаб змейки которая создаётся на уровне
    public GameObject _gameLevelElementPrefab;                     // префаб элемента уровня
    [Range(0f, 1f)]
    public float _gameLevelMotionSpeed = 1.0f;                     // скорость перемещения элементов уровня
    public float _gameBoxScaler = 4.5f;                            // множитель масштаба блоков на уровне

    public enum LevelType
    {
        infinity, limmit
    }

    public enum LevelGenerator
    {
        procedure, from_catalog
    }

    [Header("Блок настроек генерации уровня")]
    public LevelGenerator _gameLevelGeneration = LevelGenerator.procedure;
    public LevelType _gameLevelType = LevelType.infinity;
    public GameLevelSystem SetGameLevelType(LevelType type) { _gameLevelType = type; return this; }

    public int _gameLevelWidth = 6;                                // ширина игрового поля
    public int _lineBoxMaxCount = 6;                               // максимальное количество ящиков на элементе

    public int _lineBoxMinValue = 1;                               // минимальное значение для ящика
    public int _lineBoxMaxValue = 10;                              // максимальное значение для ящика

    public int _lineVisibleCount = 25;                             // количество видимых элементов уровня
    public int _lineMaxCount = 0;                                  // количество генерируемых линий
    private int _lineCurrentCount = 0;                             // счетчик текущего количества элементов

    public float _levelMinSpeed = 0.1f;                            // минимальная скорость прокрутки
    public float _levelMaxSpeed = 1.0f;                            // максимальная скорость прокрутки
    public float _levelStartSpeed = 0.4f;                          // стартовая скорость прокрутки

    public LinkedList<GameObject> _gameLevelElements = new LinkedList<GameObject>();
    public LinkedList<GameObject> _gameDissolvedElements = new LinkedList<GameObject>();

    private System.Random _classRandomizer;

    public int ELEMENTS_SIZE = 0;
    public int DESSOLVED_SIZE = 0;
    public int LINES_COUNTER = 0;

    // принимает параметры из переданной структуры
    public GameLevelSystem SetLevelConfiguration(GameConstantsKeeper.GameLevelConfig config)
    {
        _gameLevelWidth = config._gameLevelWidth; _lineBoxMaxCount = config._boxMaxCount;
        _lineBoxMinValue = config._boxMinValue; _lineBoxMaxValue = config._boxMaxValue;

        _lineVisibleCount = config._lineVisible; _lineMaxCount = config._lineMaxCount;

        // выставляем корректный флаг генерации количества эдементов
        if (Mathf.Abs(_lineMaxCount) > 0)
        {
            _gameLevelType = LevelType.limmit;
        }
        else
        {
            _gameLevelType = LevelType.infinity;
        }

        _levelMinSpeed = config._levelMinSpeed; _levelMaxSpeed = config._levelMaxSpeed; 

        _levelStartSpeed = config._LevelStartSpeed; _gameLevelMotionSpeed = _levelStartSpeed;

        return this;
    }
    // запускает новый цикл уровня по переданным параметрам
    public GameLevelSystem ConstructNewLevelSession(GameConstantsKeeper.GameLevelConfig config)
    {
        SetLevelConfiguration(config).ConstructNewLevelSession();
        return this;
    }
    // полное моментальное удаление всех элементов уровня
    public GameLevelSystem DestroyGameLevel()
    {
        // удаляем элементы в связном списке активных элементов
        while (_gameLevelElements.Count > 0)
        {
            Destroy(_gameLevelElements.First.Value);
            _gameLevelElements.RemoveFirst();
        }

        // удаляем элементы в связном списке элементов и так помеченных на удаление
        while (_gameDissolvedElements.Count > 0)
        {
            Destroy(_gameDissolvedElements.First.Value);
            _gameDissolvedElements.RemoveFirst();
        }

        return this;
    }
    // запускает новый цикл уровня по имеющимся параметрам
    public GameLevelSystem ConstructNewLevelSession()
    {
        DestroyGameLevel();            // для начала удаляем если что-то есть

        for (int i = 0; i < _lineVisibleCount; i++)
        {
            GameObject element = Instantiate(_gameLevelElementPrefab, transform) as GameObject;

            Vector3 position = element.transform.position;

            position.z = position.z - (_gameBoxScaler * i);
            element.transform.position = position;

            var elementSystem = element.GetComponent<GameLevelElementSystem>();

            elementSystem
                .SetSectorCount(_gameLevelWidth)
                .SetBoxScale(_gameBoxScaler)
                .SetBoxMinValue(_lineBoxMinValue)
                .SetBoxMaxValue(_lineBoxMaxValue)
                .SetBoxCount(_classRandomizer.Next(0, (_classRandomizer.Next(0, Mathf.Min(_lineBoxMaxCount, _gameLevelWidth) + 1))))
                .SetBorderType(GameLevelElementSystem.BorderType.Both);

            elementSystem.GameLevelElementReconstruct();

            _gameLevelElements.AddLast(element);
        }

        // сбрасываем счётчик построенных линий
        _lineCurrentCount = 0;

        return this;
    }

    private void Awake()
    {
        _classRandomizer = new System.Random();
        ConstructNewLevelSession();
        //ConstructStartElements();
    }

    void ConstructStartElements()
    {
        for (int i = 0; i < _lineVisibleCount; i++)
        {
            GameObject element = Instantiate(_gameLevelElementPrefab, transform) as GameObject;

            Vector3 position = element.transform.position;

            position.z = position.z - (_gameBoxScaler * i);
            element.transform.position = position;

            var elementSystem = element.GetComponent<GameLevelElementSystem>();

            elementSystem
                .SetSectorCount(_gameLevelWidth)
                .SetBoxScale(_gameBoxScaler)
                .SetBoxMinValue(_lineBoxMinValue)
                .SetBoxMaxValue(_lineBoxMaxValue)
                .SetBoxCount(_classRandomizer.Next(0, (_classRandomizer.Next(0, Mathf.Min(_lineBoxMaxCount, _gameLevelWidth) + 1))))
                .SetBorderType(GameLevelElementSystem.BorderType.Both);

            elementSystem.GameLevelElementReconstruct();

            _gameLevelElements.AddLast(element);
        }
    }

    private void FixedUpdate()
    {
        UpdateElementsPosition();                       // перемещение всех элементов уровня вперед согласно заданной скорости
        UpdateOutOfRangeElements();                     // удаление первого элемента списка вышедшего за область видимости
        UpdateElementsCount();                          // создание новых элементов уровня в конце списка при удалении с начала
        UpdateGameLevelSpeed();                         // обновляет значения скорости прокрутки уровня, если разрешен и есть изменения

        DESSOLVED_SIZE = _gameDissolvedElements.Count;
        ELEMENTS_SIZE = _gameLevelElements.Count;

        if (_gameLevelType == LevelType.limmit)
        {
            LINES_COUNTER = _lineCurrentCount;
        }
        else
        {
            LINES_COUNTER = 0;
        }
    }

    // перемещение всех элементов уровня вперед согласно заданной скорости
    void UpdateElementsPosition()
    {
        // метод перемещает все элементы уровня вперед согласно заданной скорости
        foreach(GameObject element in _gameLevelElements)
        {
            Vector3 position = element.transform.position;
            position.z = position.z + _gameLevelMotionSpeed;

            element.transform.position = position;
        }

        // создаем переменную в которую положим ссылку на пустое значение (после удаления элемента)
        LinkedListNode<GameObject> null_reference = null;
        // для продолжения движения растворяемых элементов вперед перебираем их список
        foreach (GameObject element in _gameDissolvedElements)
        {
            if (element != null)
            {
                // если элемент еще существует, то действуем так же как и цикле выше
                Vector3 position = element.transform.position;
                position.z = position.z + _gameLevelMotionSpeed;

                element.transform.position = position;
            }
            else
            {
                // если подошло время удаления, то запоминаем ссылку на данную ноду
                null_reference = _gameDissolvedElements.Find(element);
            }
        }

        // если у нас появилась нода на удаление
        if (null_reference != null)
        {
            // удаляем из листа растворяемых, чтобы не забивала память пустой ссылкой
            _gameDissolvedElements.Remove(null_reference);
        }
        
    }
    // удаление первого элемента списка вышедшего за область видимости
    void UpdateOutOfRangeElements()
    {
        // работает только если есть элементы
        if (_gameLevelElements.Count != 0)
        {
            Vector3 position = _gameLevelElements.First.Value.transform.position;

            if (position.z > _lineVisibleCount + _gameBoxScaler)
            {
                // вызываем удаление и растворение класса элемента уровня
                _gameLevelElements.First.Value.GetComponent<GameLevelElementSystem>().DissolveAndDestroy(1f);
                // чтобы элемент продолжал двигаться вперед, записываем его в список растворяемых элементов
                _gameDissolvedElements.AddFirst(_gameLevelElements.First.Value);
                // запускаем процесс удаления элемента уже тут, так как прошлая команда удалит только наследников
                Destroy(_gameLevelElements.First.Value, 1);
                // удаляем запись из основного листа элементов уровня
                _gameLevelElements.RemoveFirst();
            }
        }
    }
    // создание новых элементов уровня в конце списка при удалении с начала
    void UpdateElementsCount()
    {
        // сложносоставное условие, если режим лимитный, но счётчик всё еще меньше максимума для уровня
        // или режим безлимитный; и при этом количество элементов уровня меньше значения количества видимых элементов
        if (((_gameLevelType == LevelType.limmit && _lineCurrentCount < _lineMaxCount) ||
            _gameLevelType == LevelType.infinity) && _gameLevelElements.Count < _lineVisibleCount)
        {
            GameObject element = Instantiate(_gameLevelElementPrefab, transform) as GameObject;

            Vector3 position = _gameLevelElements.Last.Value.transform.position;

            position.z = position.z - _gameBoxScaler;
            element.transform.position = position;

            var elementSystem = element.GetComponent<GameLevelElementSystem>();

            elementSystem
                .SetSectorCount(_gameLevelWidth)
                .SetBoxScale(_gameBoxScaler)
                .SetBoxMinValue(_lineBoxMinValue)
                .SetBoxMaxValue(_lineBoxMaxValue)
                .SetBoxCount(_classRandomizer.Next(0, (_classRandomizer.Next(0, Mathf.Min(_lineBoxMaxCount, _gameLevelWidth) + 1))))
                .SetFoodCount(_classRandomizer.Next(0, Mathf.Min(2, _gameLevelWidth - _lineBoxMaxCount) + 1))
                .SetBorderType(GameLevelElementSystem.BorderType.Both);

            elementSystem.SolveGameLevelElement(1);

            _gameLevelElements.AddLast(element);

            // если лимитный режим, то инкремируем счётчик
            if (_gameLevelType == LevelType.limmit) _lineCurrentCount++;
        }
        // как только все линии уровня были построены
        else if (_lineCurrentCount == _lineMaxCount && _gameLevelType == LevelType.limmit)
        {
            // как только достигнут лимитный максимум создаём финиш
            GameObject element = Instantiate(_gameLevelElementPrefab, transform) as GameObject;

            Vector3 position = _gameLevelElements.Last.Value.transform.position;

            position.z = position.z - _gameBoxScaler;
            element.transform.position = position;

            var elementSystem = element.GetComponent<GameLevelElementSystem>();

            elementSystem
                .SetSectorCount(_gameLevelWidth)
                .SetBoxScale(_gameBoxScaler)
                .SetBorderType(GameLevelElementSystem.BorderType.Both);

            elementSystem.SolveGameLevelFinishLine(1);  // создаём финиш

            _gameLevelElements.AddLast(element);

            SetGameLevelType(LevelType.infinity);       // переводим в режим бесконечной генерации
        }
    }
    public void MovingLevelDown(float range)
    {
        // метод перемещает все элементы уровня вперед согласно заданной скорости
        foreach (GameObject element in _gameLevelElements)
        {
            Vector3 position = element.transform.position;
            position.z = position.z - range;

            element.transform.position = position;
        }

        foreach (GameObject element in _gameDissolvedElements)
        {
            // если элемент еще существует, то действуем так же как и цикле выше
            Vector3 position = element.transform.position;
            position.z = position.z - range;

            element.transform.position = position;
        }
    }

    // ---------------------- управление скоростью прокрутки уровня --------------------

    private float _v;                                                 // значения со стрелок
    private bool _accelerationEnable = true;                          // флаг взможности менять скрость

    public bool GetAccelerationFlag()
    {
        return _accelerationEnable;
    }
    public GameLevelSystem SetAccelerationFlag(bool enable) { _accelerationEnable = enable; return this; }

    // получение данных по горизонтали и вертикали со стрелок
    void Inputs()
    {
        _v = Input.GetAxis("Vertical");
    }
    // обновляет значения скорости прокрутки уровня, если разрешен и есть изменения
    void UpdateGameLevelSpeed()
    {
        if (_accelerationEnable)
        {
            Inputs();                              // считываем значение со стрелок

            if (_v > 0)
            {
                IncreaseLevelSpeed(_v * 0.01f);             // увеличиваем скорость
            }
            else
            {
                DecreaseLevelSpeed(_v * 0.01f);             // замедляем скорость
            }
        }
        
    }
    public void IncreaseLevelSpeed(float value)
    {
        if ((_gameLevelMotionSpeed + value) < _levelMaxSpeed)
        {
            _gameLevelMotionSpeed += value;
        }
        else
        {
            _gameLevelMotionSpeed = _levelMaxSpeed;
        }
    }
    public void DecreaseLevelSpeed(float value)
    {
        if ((_gameLevelMotionSpeed + value) > _levelMinSpeed)
        {
            _gameLevelMotionSpeed += value;
        }
        else
        {
            _gameLevelMotionSpeed = _levelMinSpeed;
        }
    }

    // обновляет скорость на изначальную
    public GameLevelSystem SetDefaultGameSpeed() { _gameLevelMotionSpeed = _levelStartSpeed; return this; }
    // назначает текущую скорость прокрутки уровня
    public GameLevelSystem SetCurrenGameSpeed(float speed) { _gameLevelMotionSpeed = speed; return this; }
}