using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Rendering.FilterWindow;

public class GameLevelElementSystem : MonoBehaviour
{

    public GameObject _elementBorderPrefab;                 // объект границы элемента уровня
    public GameObject _elementSectorPrefab;                 // объект сектора поля уровня
    public GameObject _elementBoxPrefab;                    // объект блок игрового поля


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
    public int _elementBoxCout = 0;                         // количество блоков на элементе уровня
    public int GetBoxCount() { return _elementBoxCout; }
    public GameLevelElementSystem SetBoxCount(int count) { _elementBoxCout = count; return this; }

    public float _elementBoxScale = 0;                      // масштаб блоков на элементе уровня
    public float GetBoxScale() { return _elementBoxScale; }
    public GameLevelElementSystem SetBoxScale(float scale) { _elementBoxScale = scale; return this; }

    // ------------------------------------- блок дополнительных внутренних полей класса ---------------------------

    private List<GameObject> _elementBorders = new List<GameObject>();         // лист с созданными границами
    private List<GameObject> _elementSectors = new List<GameObject>();         // лист с созданными секторами
    private List<GameObject> _elementBoxes = new List<GameObject>();           // лист с созданными блоками
    public List<float> _elementsXpositions = new List<float>();                // лист с позициями по оси X
    private List<bool> _elementSectorsClear = new List<bool>();                // булевый лист с флагом пустоты сектора

    private Vector3 _borderGeometryScaler = Vector3.zero;                      // скалер размеров границы
    private Vector3 _sectorGeometryScaler = Vector3.zero;                      // скалер размеров сектора

    private void UpdatePrefabsScalers()
    {
        if (_elementBorderPrefab != null)
             _borderGeometryScaler = _elementBorderPrefab.GetComponent<Transform>().localScale;

        if (_elementSectorPrefab != null)
            _sectorGeometryScaler = _elementSectorPrefab.GetComponent<Transform>().localScale;
    }

    private void Awake()
	{
        UpdatePrefabsScalers();                             // обновляет информацию о скалярах установленных префабов
        SectorsConstruct();                                 // конструирует сектора элемента уровня
        BordersConstruct();                                 // конструирует границы элемента уровня
        BoxesConstruct();                                   // конструирует ящики на элементе уровня
    }

    public void GameLevelReconstruct()
    {
        DestroyAllElements();
        SectorsConstruct();                                 // конструирует сектора элемента уровня
        BordersConstruct();                                 // конструирует границы элемента уровня
        BoxesConstruct();                                   // конструирует ящики на элементе уровня
    }

    private void SectorsConstruct()
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

    private void BordersConstruct()
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
                BorderMaker(left_x_position, border_level);
                break;

            case BorderType.Right:
                BorderMaker(right_x_position, border_level);
                break;

            case BorderType.Both:
                BorderMaker(left_x_position, border_level);
                BorderMaker(right_x_position, border_level);
                break;
        }
    }

    private void BoxesConstruct()
    {
        System.Random random = new System.Random();

        for (int i = 0; i < _elementBoxCout; ++i)
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
            BoxSystem.SetBoxScore(random.Next(1, 25));

            // записываем элемент в список
            _elementBoxes.Add(box);
        }
    }

    private void BorderMaker(float x_position, float y_position) 
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
        // записываем элемент в список
        _elementBorders.Add(border);
    }

    private void DestroyAllElements()
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
    }
}