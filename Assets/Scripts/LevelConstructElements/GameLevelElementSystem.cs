using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Rendering.FilterWindow;

public class GameLevelElementSystem : MonoBehaviour
{

    public GameObject _elementBorderPrefab;                 // объект границы элемента уровня
    public GameObject _elementSectorPrefab;                 // объект сектора поля уровня


    public enum BorderType
    {
        None, Left, Right, Both
    }

    public BorderType _borderType = BorderType.None;        // выбор типа границ элемента уровня

    public BorderType GetBorderType() { return _borderType; }
    public GameLevelElementSystem SetBorderType(BorderType borderType) { _borderType = borderType; return this; }


    public int _elementSectorCount = 6;                     // количество секторов на элементе уровня
    public int GetSectorCount() { return _elementSectorCount; }
    public GameLevelElementSystem SetSectorCount(int count) { _elementSectorCount = count; return this; }

    // ------------------------------------- блок дополнительных внутренних полей класса ---------------------------

    private List<GameObject> _elementBorders = new List<GameObject>();         // лист с созданными границами
    private List<GameObject> _elementSectors = new List<GameObject>();         // лист с созданными секторами

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
    }

    private void SectorsConstruct()
    {
        // берем крайнюю правую координату для локальной позиции сектора
        float right_x_position = _sectorGeometryScaler.x * (_elementSectorCount - 1) / 2;

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
}