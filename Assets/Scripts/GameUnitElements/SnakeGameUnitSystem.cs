using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeGameUnitSystem : MonoBehaviour
{

    public enum SnakeUnitMode
    {
        Empty, Test, Default, Procedure
    }

    public SnakeUnitMode _snakeUnitMode = SnakeUnitMode.Default;

    public GameObject _snakeElementPrefab;

    public List<GameObject> _snakeElements = new List<GameObject>();

    public float _elementScaler = 1.0f;

    private GameObject _snakeGameUnit;

    // Чё-то глючит гит на VS
    public Vector3 CURRENT_HEAD_LOCK_POS;

    private void Awake()
    {
        _snakeGameUnit = GetComponent<GameObject>();

        switch (_snakeUnitMode)
        {
            case SnakeUnitMode.Empty:
                break;

            case SnakeUnitMode.Test:
                TestSnakeConstruct();
                break;


            case SnakeUnitMode.Default:
                break;

            case SnakeUnitMode.Procedure:
                break;

        }
    }

    private void FixedUpdate()
    {
        UpdateSnakeElementsPosition();
    }

    // добавляет новый элемент в змею
    public void AddSnakeElement(ref GameObject snakeElement)
    {
        _snakeElements.Add(snakeElement);
    }

    // удаляет крайний элемент змеи
    public void RemoveLastSnakeElements()
    {
        if (GetElementsCount() != 0)
        {
            _snakeElements.RemoveAt(GetElementsCount() - 1);
        }
    }

    // возвращет количество элементов в змее
    public int GetElementsCount()
    {
        return _snakeElements.Count;
    }

    public void SetElementsScale(float scale)
    {
        if (scale != _elementScaler)
        {
            _elementScaler = scale; // назначаем новый элемент

            for (int i = 0; i < GetElementsCount(); ++i)
            {
                // пролучаем систему элемента змеи
                var system = _snakeElements[i].GetComponent<SnakeElementSystem>();
                // устанавливаем скалер
                system.SetElementScale(_elementScaler);
            }
        }
    }

    // конструктор тестовой змейки
    private void TestSnakeConstruct()
    {
        for (int i = 0; i < 17; ++i)
        {
            // создаём объект элемента змеи в нулевом трансформе
            GameObject snake_element = Instantiate(_snakeElementPrefab, transform) as GameObject;

            if (i == 0)
            {
                // первый элемент создаётся в нулевой локальной точке и на 25% больше остальных
                snake_element.GetComponent<SnakeElementSystem>()
                    .SetElementScale(_elementScaler * 1.25f)
                    .SetElementType(SnakeElementSystem.SnakeElementType.Head);
                // добавляем элемент в список звеньев
                _snakeElements.Add(snake_element);
            }
            else
            {
                // создаём элементы звеньев
                snake_element.GetComponent<SnakeElementSystem>()
                    .SetElementScale(_elementScaler)
                    .SetElementType(SnakeElementSystem.SnakeElementType.Link);

                // для определения стартового положения n-ого элемента берем локальный трансформ предыдущего
                Vector3 element_position = _snakeElements[i - 1].transform.localPosition;

                if (i == 1)
                {
                    // если текущий индекс первый после головы, то смещение берется с поправкой на скалер умноженный на 112,5%
                    element_position.z += (_snakeElements[i - 1].GetComponent<SnakeElementSystem>().GetElementScaler() * 1.125f);
                }
                else
                {
                    // все последующие элементы, берутся с поправкой на скалер умноженный на 125%
                    element_position.z += (_snakeElements[i - 1].GetComponent<SnakeElementSystem>().GetElementScaler() * 1.25f);
                }

                // задаём локальную позицию элемента в змее
                snake_element.transform.localPosition = element_position;
                // добавляем элемент в список звеньев
                _snakeElements.Add(snake_element);
            }
        }
    }

    // обновление позиций элементов змейки по головному элементу
    private void UpdateSnakeElementsPosition()
    {
        if (GetElementsCount() > 0)
        {
            Vector3 head_local_position = _snakeElements[0].transform.localPosition;
            CURRENT_HEAD_LOCK_POS = head_local_position;

            int count = GetElementsCount();
            float acceleration = 1 + (count * 0.1f);


            for (int i = 0; i < count; ++i)
            {
                // берем позицию текущего элемента
                Vector3 element_local_position = _snakeElements[i].transform.localPosition;
                // считаем разницу позиции элемента по оси X
                float delta_x = head_local_position.x - element_local_position.x;

                if (Mathf.Abs(delta_x) > 0)
                {
                    element_local_position.x += delta_x / Mathf.Max(1, ((count - ((count - i) - 1)) / acceleration));
                }

                _snakeElements[i].transform.localPosition = element_local_position;

            }
        }
    }
}
