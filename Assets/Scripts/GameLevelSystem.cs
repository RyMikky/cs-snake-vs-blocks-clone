using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelSystem : MonoBehaviour
{

    public GameObject _gameLevelElementPrefab;                     // префаб элемента уровня
    [Range(0f, 1f)]
    public float _gameLevelMotionSpeed = 1.0f;                     // скорость перемещения элементов уровня
    public float _gameBoxScaler = 4.5f;                            // множитель масштаба блоков на уровне

    public int _gameLevelElementsVisibleCount = 25;                // количество видимых элементов уровня
    public int _emptyLevelElementOffset = 5;                       // поличество "пустых" элементов между элементами с блоками


    public LinkedList<GameObject> _gameLevelElements = new LinkedList<GameObject>();

    private System.Random _classRandomizer;

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
                .SetBoxScale(_gameBoxScaler)
                .SetBoxCount(_classRandomizer.Next(0, (_classRandomizer.Next(0, 6))))
                .SetBorderType(GameLevelElementSystem.BorderType.Both);

            elementSystem.GameLevelReconstruct();

            _gameLevelElements.AddLast(element);
        }
    }

    private void FixedUpdate()
    {
        UpdateElementsPosition();                       // перемещаение всех элементов уровня вперед согласно заданной скорости
        UpdateOutOfRangeElements();                     // удаление первого элемента списка вышедшего за область видимости
        UpdateElementsCount();                          // создание новых элементов уровня в конце списка при удалении с начала
    }

    void UpdateElementsPosition()
    {
        // метод перемещает все элементы уровня вперед согласно заданной скорости
        foreach(GameObject element in _gameLevelElements)
        {
            Vector3 position = element.transform.position;
            position.z = position.z + _gameLevelMotionSpeed;

            element.transform.position = position;
        }
    }

    void UpdateOutOfRangeElements()
    {
        Vector3 position = _gameLevelElements.First.Value.transform.position;

        if (position.z > _gameLevelElementsVisibleCount + _gameBoxScaler)
        {
            Destroy(_gameLevelElements.First.Value);

            _gameLevelElements.RemoveFirst();
        }
    }

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
                .SetBoxScale(_gameBoxScaler)
                .SetBoxCount(_classRandomizer.Next(0, (_classRandomizer.Next(0, 6))))
                .SetBorderType(GameLevelElementSystem.BorderType.Both);

            elementSystem.GameLevelReconstruct();

            _gameLevelElements.AddLast(element);
        }
    }
}
