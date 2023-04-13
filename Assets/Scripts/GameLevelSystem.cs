using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelSystem : MonoBehaviour
{

    public GameObject _snakeUnitPrefab;                            // префаб змейки которая создаётся на уровне
    public GameObject _gameLevelElementPrefab;                     // префаб элемента уровня
    [Range(0f, 1f)]
    public float _gameLevelMotionSpeed = 1.0f;                     // скорость перемещения элементов уровня
    public float _gameBoxScaler = 4.5f;                            // множитель масштаба блоков на уровне

    public enum LevelGenerator
    {
        procedure, from_catalog
    }

    [Header("Блок настроек генерации уровня")]
    public LevelGenerator _gameLevelGeneration = LevelGenerator.procedure;
    public int _gameLevelElementsVisibleCount = 25;                // количество видимых элементов уровня
    public int _gameLevelWidth = 6;                                // ширина игрового поля

    public int _levelElementBoxMaxCount = 6;                       // максимальное количество ящиков на элементе

    public int _levelElementBoxMinValue = 1;                       // минимальное значение для ящика
    public int _levelElementBoxMaxValue = 10;                      // максимальное значение для ящика


    public LinkedList<GameObject> _gameLevelElements = new LinkedList<GameObject>();
    public LinkedList<GameObject> _gameDissolvedElements = new LinkedList<GameObject>();

    private System.Random _classRandomizer;

    public int ELEMENTS_SIZE = 0;
    public int DESSOLVED_SIZE = 0;

    private void Awake()
    {
        _classRandomizer = new System.Random();
        ConstructStartElements();
    }

    void ConstructStartElements()
    {
        for(int i = 0; i < _gameLevelElementsVisibleCount; i++)
        {
            GameObject element = Instantiate(_gameLevelElementPrefab, transform) as GameObject;

            Vector3 position = element.transform.position;

            position.z = position.z - (_gameBoxScaler * i);
            element.transform.position = position;

            var elementSystem = element.GetComponent<GameLevelElementSystem>();

            elementSystem
                .SetSectorCount(_gameLevelWidth)
                .SetBoxScale(_gameBoxScaler)
                .SetBoxMinValue(_levelElementBoxMinValue)
                .SetBoxMaxValue(_levelElementBoxMaxValue)
                .SetBoxCount(_classRandomizer.Next(0, (_classRandomizer.Next(0, Mathf.Min(_levelElementBoxMaxCount, _gameLevelWidth) + 1))))
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

        DESSOLVED_SIZE = _gameDissolvedElements.Count;
        ELEMENTS_SIZE = _gameLevelElements.Count;
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
        Vector3 position = _gameLevelElements.First.Value.transform.position;

        if (position.z > _gameLevelElementsVisibleCount + _gameBoxScaler)
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
    // создание новых элементов уровня в конце списка при удалении с начала
    void UpdateElementsCount()
    {
        if (_gameLevelElements.Count < _gameLevelElementsVisibleCount)
        {
            GameObject element = Instantiate(_gameLevelElementPrefab, transform) as GameObject;

            Vector3 position = _gameLevelElements.Last.Value.transform.position;

            position.z = position.z - _gameBoxScaler;
            element.transform.position = position;

            var elementSystem = element.GetComponent<GameLevelElementSystem>();

            elementSystem
                .SetSectorCount(_gameLevelWidth)
                .SetBoxScale(_gameBoxScaler)
                .SetBoxMinValue(_levelElementBoxMinValue)
                .SetBoxMaxValue(_levelElementBoxMaxValue)
                .SetBoxCount(_classRandomizer.Next(0, (_classRandomizer.Next(0, Mathf.Min(_levelElementBoxMaxCount, _gameLevelWidth) + 1))))
                .SetFoodCount(_classRandomizer.Next(0, Mathf.Min(2, _gameLevelWidth - _levelElementBoxMaxCount) + 1))
                .SetBorderType(GameLevelElementSystem.BorderType.Both);

            elementSystem.SolveGameLevelElement(1);

            _gameLevelElements.AddLast(element);
        }
    }
}