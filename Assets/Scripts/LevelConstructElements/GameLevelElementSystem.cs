using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Rendering.FilterWindow;

public class GameLevelElementSystem : DissolvableObject
{

    public GameObject _elementBorderPrefab;                 // объект границы элемента уровня
    public GameObject _elementSectorPrefab;                 // объект сектора поля уровня
    public GameObject _elementBoxPrefab;                    // объект блок игрового поля
    public GameObject _elementSnakePrefab;                  // объект звено змейки

    public enum ActivationMode
    {
        fromGameLevel, ManualDebug
    }
    // тип работы скрипта, определяет создание элементов в Awake
    public ActivationMode _activationMode = ActivationMode.ManualDebug; 

    public enum BorderType
    {
        None, Left, Right, Both
    }

    public BorderType _borderType = BorderType.None;        // выбор типа границ элемента уровня

    public BorderType GetBorderType() { return _borderType; }
    public GameLevelElementSystem SetBorderType(BorderType borderType) { _borderType = borderType; return this; }

    [Header("Блок настроек секторов на элементе")]
    public int _elementSectorCount = 6;                     // количество секторов на элементе уровня
    public int GetSectorCount() { return _elementSectorCount; }
    public GameLevelElementSystem SetSectorCount(int count) { _elementSectorCount = count; return this; }


    [Header("Блок настроек ящиков на элементе")]
    public int _elementBoxCount = 0;                        // количество блоков на элементе уровня
    public int GetBoxCount() { return _elementBoxCount; }
    public GameLevelElementSystem SetBoxCount(int count) { _elementBoxCount = count; return this; }

    public int _elementFoodCount = 0;                       // количество "еды" на элементе уровня
    public int GetFoodCount() { return _elementFoodCount; }
    public GameLevelElementSystem SetFoodCount(int count) { _elementFoodCount = count; return this; }

    public float _elementBoxScale = 0;                      // масштаб блоков на элементе уровня
    public float GetBoxScale() { return _elementBoxScale; }
    public GameLevelElementSystem SetBoxScale(float scale) { _elementBoxScale = scale; return this; }

    public int _elementBoxMinValue = 1;                     // минимальное количество очков блока
    public int GetBoxMinValue() { return _elementBoxMinValue; }
    public GameLevelElementSystem SetBoxMinValue(int value) { _elementBoxMinValue = value; return this; }

    public int _elementBoxMaxValue = 0;                     // максимальное количество очков блока
    public int GetBoxMaxValue() { return _elementBoxMaxValue; }
    public GameLevelElementSystem SetBoxMaxValue(int value) { _elementBoxMaxValue = value; return this; }

    // ------------------------------------- блок дополнительных внутренних полей класса ---------------------------

    private List<GameObject> _elementBorders = new List<GameObject>();         // лист с созданными границами
    private List<GameObject> _elementSectors = new List<GameObject>();         // лист с созданными секторами
    private List<GameObject> _elementBoxes = new List<GameObject>();           // лист с созданными блоками
    private List<GameObject> _elementFood = new List<GameObject>();            // лист с созданной "едой"
    public List<float> _elementsXpositions = new List<float>();                // лист с позициями по оси X
    private List<bool> _elementSectorsClear = new List<bool>();                // булевый лист с флагом пустоты сектора

    private Vector3 _borderGeometryScaler = Vector3.zero;                      // скалер размеров границы
    private Vector3 _sectorGeometryScaler = Vector3.zero;                      // скалер размеров сектора
    private System.Random _random = new System.Random();

    private GameObject _baseLevelElementGameObject;

    private void UpdatePrefabsScalers()
    {
        _baseLevelElementGameObject = GetComponent<GameObject>();

        if (_elementBorderPrefab != null)
             _borderGeometryScaler = _elementBorderPrefab.GetComponent<Transform>().localScale;

        if (_elementSectorPrefab != null)
            _sectorGeometryScaler = _elementSectorPrefab.GetComponent<Transform>().localScale;
    }

    private void Awake()
	{
        UpdatePrefabsScalers();                             // обновляет информацию о скалярах установленных префабов

        if (_activationMode == ActivationMode.ManualDebug)
        {
            SectorsConstruct(0);                                 // конструирует сектора элемента уровня
            BordersConstruct(0);                                 // конструирует границы элемента уровня
            BoxesConstruct(0);                                   // конструирует ящики на элементе уровня
            FoodConstruct(0);                                    // конструирует "еду" на элементе уровня
        }
    }

    // реконструирует элементы уровня по заданым заранее параметрам 
    public void GameLevelElementReconstruct()
    {
        DestroyAllElements();
        SectorsConstruct(0);                                // конструирует сектора элемента уровня
        BordersConstruct(0);                                // конструирует границы элемента уровня
        BoxesConstruct(0);                                  // конструирует ящики на элементе уровня
        FoodConstruct(0);                                   // конструирует "еду" на элементе уровня
    }
    // общая функция проециорвания всех элементов уровня по заданым заранее параметрам 
    public void SolveGameLevelElement(float time)
    {
        DestroyAllElements();                               // на всякий пожарный
        SectorsConstruct(time);                             // конструирует сектора элемента уровня
        BordersConstruct(time);                             // конструирует границы элемента уровня
        BoxesConstruct(time);                               // конструирует ящики на элементе уровня
        FoodConstruct(time);                                // конструирует "еду" на элементе уровня
    }

    // проецирует на элемент карты напольные сектора с заданым временем появления
    private void SectorsConstruct(float time)
    {
        // берем крайнюю правую координату для локальной позиции сектора
        float right_x_position = _sectorGeometryScaler.x * (_elementSectorCount - 1) / 2;
        // добавляем позицию в список и ставим флаг пустоты
        _elementsXpositions.Add(right_x_position); _elementSectorsClear.Add(true);

        for (int i = 0; i < _elementSectorCount; ++i)
        {
            GameObject element = Instantiate(_elementSectorPrefab, transform) as GameObject;

            // берем локальный трансформ созданного элемента
            Vector3 element_position = element.transform.localPosition;
            // задаём координату по X из ранее полученного значения
            element_position.x = right_x_position;
            // перезаписываем положение элемента
            element.transform.localPosition = element_position;

            if (time > 0)
            {
                // если задано время то "проявляем объект"
                float minTime = time * (1 - 0.2f);       // минимальное время в диапазоне time - 20%
                float maxTime = time * (1 + 0.2f);       // максимальное время в диапазоне time + 20%

                element.GetComponent<DissolvableObject>()
                    .StartSolving((float)(_random.NextDouble() * (maxTime - minTime) + minTime));
            }

            // записываем элемент в список
            _elementSectors.Add(element);

            // декремируем позицию для следующего элемента
            right_x_position -= _sectorGeometryScaler.x;

            if (i < _elementSectorCount - 1)
            {
                // добавляем позицию в список и ставим флаг пустоты
                _elementsXpositions.Add(right_x_position); _elementSectorsClear.Add(true);
            }
        }
    }
    // проецирует на элемент карты границы уровня с заданым временем появления
    private void BordersConstruct(float time)
    {
        if (_borderType == BorderType.None) return;

        // берем крайнюю правую координату для локальной позиции сектора и прибавляем половину от скалера сектора и половину от скалера границы
        float right_x_position = (_sectorGeometryScaler.x * (_elementSectorCount - 1) / 2) 
            + (_sectorGeometryScaler.x / 2) + (_borderGeometryScaler.x / 2);

        // крайняя левая координата равняется правой с отрицательным знаком
        float left_x_position = -right_x_position;
        // возвышение граничного элемента
        float border_level = (_borderGeometryScaler.y - _sectorGeometryScaler.y) / 2;

        switch (_borderType) 
        {
            case BorderType.Left:
                BorderMaker(left_x_position, border_level, time);
                break;

            case BorderType.Right:
                BorderMaker(right_x_position, border_level, time);
                break;

            case BorderType.Both:
                BorderMaker(left_x_position, border_level, time);
                BorderMaker(right_x_position, border_level, time);
                break;
        }
    }
    // проецирует на элемент карты ящики с заданым временем появления
    private void BoxesConstruct(float time)
    {
        System.Random random = new System.Random();

        for (int i = 0; i < _elementBoxCount; ++i)
        {
            int position = 0;               // создаём переменную для определения позиции установки блока

            bool _randomProcess = true;     // задаём флаг поиска места установки блока
            while (_randomProcess)          // запускаем цикл по данному флагу
            {
                // получаем рандомное число от нуля до количества элементов
                position = random.Next(0, _elementSectorsClear.Count);

                // если нашли свободную по булевому массиву позицию
                if (_elementSectorsClear[position] == true)
                {
                    // "закрываем" данную позицию
                    _elementSectorsClear[position] = false;
                    // завершаем цикл и переходим у установке
                    _randomProcess = false;
                }
            }

            // создаём новый элемент ящик
            GameObject box = Instantiate(_elementBoxPrefab, transform) as GameObject;
            // берем локальный трансформ созданного элемента
            Vector3 element_position = box.transform.localPosition;
            // задаём координату по X из ранее полученного значения
            element_position.x = _elementsXpositions[position];
            // задаём координату по Y
            element_position.y = (_elementBoxScale / 2) + (_sectorGeometryScaler.y / 2);
            // переназначаем трансформ
            box.transform.localPosition = element_position;

            var BoxSystem = box.GetComponent<ElementBoxSystem>();
            BoxSystem.SetBoxScaler(_elementBoxScale);

            if (time > 0)
            {
                // если задано время то "проявляем объект"
                float minTime = time * (1 - 0.2f);       // минимальное время в диапазоне time - 20%
                float maxTime = time * (1 + 0.2f);       // максимальное время в диапазоне time + 20%

                BoxSystem.StartSolving((float)(_random.NextDouble() * (maxTime - minTime) + minTime));
            }

            // генерируем значение очков ящика
            int BoxValue = random.Next(_elementBoxMinValue, _elementBoxMaxValue + 1);
            if ( BoxValue <= 0)
            {
                // если так получилось, что меньше или равно нулю, то просто удаляем этот ящик
                Destroy(box);
            }
            else
            {
                // если со значением всё окей
                BoxSystem.SetBoxValue(BoxValue);
                // записываем элемент в список
                _elementBoxes.Add(box);
            }
        }
    }
    // проецирует на элемент карты еду для змейки с заданым временем появления
    private void FoodConstruct(float time)
    {
        System.Random random = new System.Random();

        for (int i = 0; i < _elementFoodCount; ++i)
        {
            int position = 0;               // создаём переменную для определения позиции установки "еды"

            bool _randomProcess = true;     // задаём флаг поиска места установки блока
            while (_randomProcess)          // запускаем цикл по данному флагу
            {
                // получаем рандомное число от нуля до количества элементов
                position = random.Next(0, _elementSectorsClear.Count);

                // если нашли свободную по булевому массиву позицию
                if (_elementSectorsClear[position] == true)
                {
                    // "закрываем" данную позицию
                    _elementSectorsClear[position] = false;
                    // завершаем цикл и переходим у установке
                    _randomProcess = false;
                }
            }

            // создаём новый элемент ящик
            GameObject food = Instantiate(_elementSnakePrefab, transform) as GameObject;
            // берем локальный трансформ созданного элемента
            Vector3 element_position = food.transform.localPosition;
            // задаём координату по X из ранее полученного значения
            element_position.x = _elementsXpositions[position];
            // задаём координату по Y
            element_position.y = (_elementBoxScale / 2) + (_sectorGeometryScaler.y / 2);
            // переназначаем трансформ
            food.transform.localPosition = element_position;
            // назначаем tag еды змеи
            food.tag = "SnakeFood";
            var SnakeSystem = food.GetComponent<SnakeElementSystem>();
            SnakeSystem
                .SetGameLevelElementSystem(this)
                .SetGameLevelElementSystemGO(ref _baseLevelElementGameObject)
                .SetElementType(SnakeElementSystem.SnakeElementType.Food);

            if (time > 0)
            {
                // если задано время то "проявляем объект"
                float minTime = time * (1 - 0.2f);       // минимальное время в диапазоне time - 20%
                float maxTime = time * (1 + 0.2f);       // максимальное время в диапазоне time + 20%

                SnakeSystem.StartSolving((float)(_random.NextDouble() * (maxTime - minTime) + minTime));
            }

            // записываем элемент в список
            _elementFood.Add(food);
        }
    }
    // создает границу уровня по заданым координатам и времени появления
    private void BorderMaker(float x_position, float y_position, float time) 
    {
        GameObject border = Instantiate(_elementBorderPrefab, transform) as GameObject;

        // берем локальный трансформ созданного элемента
        Vector3 element_position = border.transform.localPosition;
        // задаём координату по X из полученного значения
        element_position.x = x_position;
        // задаём координату по Y из полученного значения
        element_position.y = y_position;
        // перезаписываем положение элемента
        border.transform.localPosition = element_position;

        if (time > 0)
        {
            // если задано время то "проявляем объект"
            float minTime = time * (1 - 0.2f);       // минимальное время в диапазоне time - 20%
            float maxTime = time * (1 + 0.2f);       // максимальное время в диапазоне time + 20%

            border.GetComponent<DissolvableObject>()
                .StartSolving((float)(_random.NextDouble() * (maxTime - minTime) + minTime));
        }

        // записываем элемент в список
        _elementBorders.Add(border);
    }

    // удаляет ящик с элемента уровня
    public void DestroyBoxElement(float time, ref GameObject element)
    {
        float minTime = time * (1 - 0.1f);       // минимальное время в диапазоне time - 20%
        float maxTime = time * (1 + 0.1f);       // максимальное время в диапазоне time + 20%

        if (_elementBoxes.Count != 0 && _elementBoxes.Contains(element))
        {
            int index = 0;

            for (int i = 0; i < _elementBoxes.Count; i++)
            {
                if (_elementBoxes[i] == element)
                {
                    index = i; break;
                }
            }

            _elementBoxes[index].GetComponent<DissolvableObject>()
                    .StartDissolving((float)(_random.NextDouble() * (maxTime - minTime) + minTime));
            Destroy(element, (float)(_random.NextDouble() * (maxTime - minTime) + minTime));
            _elementBoxes.Remove(element);
        }
        else
        {
            if (element == null)
            {
                Debug.Log("GameLevelElementSystem::DestroyBoxElement::element == null");
            }
        }
    }
    // удаляет элемент еды с уровня
    public void DestroyFoodElement(float time, ref GameObject element)
    {
        float minTime = time * (1 - 0.1f);       // минимальное время в диапазоне time - 20%
        float maxTime = time * (1 + 0.1f);       // максимальное время в диапазоне time + 20%

        if (_elementFood.Count != 0 && _elementFood.Contains(element))
        {
            int index = 0;

            for (int i = 0; i < _elementFood.Count; i++)
            {
                if (_elementFood[i] == element)
                {
                    index = i; break;
                }
            }

            _elementFood[index].GetComponent<DissolvableObject>()
                    .StartDissolving((float)(_random.NextDouble() * (maxTime - minTime) + minTime));
            Destroy(element, (float)(_random.NextDouble() * (maxTime - minTime) + minTime));
            _elementFood.Remove(element);
        }
        else
        {
            if (element == null)
            {
                Debug.Log("GameLevelElementSystem::DestroyFoodElement::element == null");
            }
        }
    }
    // просто уничтожает все элементы
    public void DestroyAllElements()
    {
        if (_elementBorders.Count != 0)
        {
            for (int i = 0; i < _elementBorders.Count; i++)
            {
                Destroy(_elementBorders[i]);
            }

            _elementBorders.Clear();
        }

        if (_elementSectors.Count != 0)
        {
            for (int i = 0; i < _elementSectors.Count; i++)
            {
                Destroy(_elementSectors[i]);
            }

            _elementSectors.Clear();
        }

        if (_elementBoxes.Count != 0)
        {
            for (int i = 0; i < _elementBoxes.Count; i++)
            {
                Destroy(_elementBoxes[i]);
            }

            _elementBoxes.Clear();
        }

        if (_elementFood.Count != 0)
        {
            for (int i = 0; i < _elementFood.Count; i++)
            {
                Destroy(_elementFood[i]);
            }

            _elementFood.Clear();
        }
    }
    // растворяет и уничтожает все элементы
    public void DissolveAndDestroy(float time)
    {
        float minTime = time * (1 - 0.2f);       // минимальное время в диапазоне time - 20%
        float maxTime = time * (1 + 0.2f);       // максимальное время в диапазоне time + 20%

        if (_elementBorders.Count != 0)
        {
            for (int i = 0; i < _elementBorders.Count; i++)
            {
                _elementBorders[i].GetComponent<DissolvableObject>()
                    .StartDissolving((float)(_random.NextDouble() * (maxTime - minTime) + minTime));
                Destroy(_elementBorders[i], (float)(_random.NextDouble() * (maxTime - minTime) + minTime));
            }

            _elementBorders.Clear();
        }

        if (_elementSectors.Count != 0)
        {
            for (int i = 0; i < _elementSectors.Count; i++)
            {
                _elementSectors[i].GetComponent<DissolvableObject>()
                    .StartDissolving((float)(_random.NextDouble() * (maxTime - minTime) + minTime));
                Destroy(_elementSectors[i], (float)(_random.NextDouble() * (maxTime - minTime) + minTime));
            }

            _elementSectors.Clear();
        }

        if (_elementBoxes.Count != 0)
        {
            for (int i = 0; i < _elementBoxes.Count; i++)
            {
                _elementBoxes[i].GetComponent<ElementBoxSystem>()
                    .ShowBoxValue(false)
                    .StartDissolving((float)(_random.NextDouble() * (maxTime - minTime) + minTime));
                Destroy(_elementBoxes[i], (float)(_random.NextDouble() * (maxTime - minTime) + minTime));
            }

            _elementBoxes.Clear();
        }

        if (_elementFood.Count != 0)
        {
            for (int i = 0; i < _elementFood.Count; i++)
            {
                _elementFood[i].GetComponent<DissolvableObject>()
                    .StartDissolving((float)(_random.NextDouble() * (maxTime - minTime) + minTime));
                Destroy(_elementFood[i], (float)(_random.NextDouble() * (maxTime - minTime) + minTime));
            }

            _elementFood.Clear();
        }
    }
}