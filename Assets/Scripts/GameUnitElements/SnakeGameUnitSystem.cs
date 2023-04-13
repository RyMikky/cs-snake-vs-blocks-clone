using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Rendering.FilterWindow;

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
    public int _maxVisibleLinks = 6;
    private int _invisibleLinks = 0;

    private GameObject _snakeHeadGO;
    private SnakeElementSystem _snakeHeadSystem;

    private GameObject _snakeGameUnit;

    private System.Random _random = new System.Random();

    // Чё-то глючит гит на VS
    public Vector3 CURRENT_HEAD_LOCK_POS;

    private void Awake()
    {
        _snakeGameUnit = gameObject;

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
        UpdateLinksCount();                        // обновляет количество элементов на голове змеи
    }

    // обновляет количество элементов на голове змеи
    private void UpdateLinksCount()
    {
        if(_snakeElements.Count + _invisibleLinks> 0)
        {
            _snakeHeadSystem.SetLinksCount(_snakeElements.Count + _invisibleLinks);
        }
    }


    // добавляет новый элемент в змею
    public void AddSnakeNewLink()
    {
        // если максимум видимых звеньев не превышен
        if (_snakeElements.Count < _maxVisibleLinks)
        {
            // берем индекс крайнего элемента в массиве звеньев
            int back_index = _snakeElements.Count - 1;
            // создаём объект элемента змеи в трансформе крайнего элемента
            GameObject snake_element = Instantiate(_snakeElementPrefab, transform) as GameObject;
            // создаём элемент звена змейки
            snake_element.GetComponent<SnakeElementSystem>()
                .SetElementScale(_elementScaler)
                .SetElementType(SnakeElementSystem.SnakeElementType.Link)
                .StartSolving();

            // назначаем tag балды змеи
            snake_element.tag = "SnakeLink";
            // для определения стартового положения n-ого элемента берем локальный трансформ предыдущего
            Vector3 element_position = _snakeElements[back_index].transform.localPosition;

            if (back_index == 0)
            {
                // если текущий индекс первый после головы, то смещение берется с поправкой на скалер умноженный на 112,5%
                element_position.z += (_snakeElements[back_index].GetComponent<SnakeElementSystem>().GetElementScaler() * 1.125f);
            }
            else
            {
                // все последующие элементы, берутся с поправкой на скалер умноженный на 125%
                element_position.z += (_snakeElements[back_index].GetComponent<SnakeElementSystem>().GetElementScaler() * 1.25f);
            }

            // задаём локальную позицию элемента в змее
            snake_element.transform.localPosition = element_position;
            // добавляем элемент в список звеньев
            _snakeElements.Add(snake_element);
        }
        else
        {
            // иначе просто инкремируем "виртуальное" звено
            _invisibleLinks++;
        }
        
    }
    // удаляет крайний элемент змеи
    public void RemoveLastSnakeElements(float time)
    {
        // если нет "виртуальных" звеньев то удаляем существующее звено с конца
        if (_invisibleLinks == 0)
        {
            float minTime = time * (1 - 0.1f);       // минимальное время в диапазоне time - 20%
            float maxTime = time * (1 + 0.1f);       // максимальное время в диапазоне time + 20%

            if (GetElementsCount() != 0)
            {
                _snakeElements[_snakeElements.Count - 1].GetComponent<DissolvableObject>()
                        .StartDissolving((float)(_random.NextDouble() * (maxTime - minTime) + minTime));
                Destroy(_snakeElements[_snakeElements.Count - 1], (float)(_random.NextDouble() * (maxTime - minTime) + minTime));
                _snakeElements.RemoveAt(_snakeElements.Count - 1);
            }
        }
        else
        {
            // иначе просто декремируем "виртуальные" звенья
            _invisibleLinks--;
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
        for (int i = 0; i < _maxVisibleLinks; ++i)
        {
            // создаём объект элемента змеи в нулевом трансформе
            GameObject snake_element = Instantiate(_snakeElementPrefab, transform) as GameObject;
            // получаем экземпляр подсистемы элемента
            SnakeElementSystem link_system = snake_element.GetComponent<SnakeElementSystem>();

            if (i == 0)
            {
                _snakeHeadGO = snake_element;                               // назначаем голову
                _snakeHeadSystem = link_system;                     // назначаем систему головы

                // первый элемент создаётся в нулевой локальной точке и на 25% больше остальных
                link_system
                    .SetGameUnitSystem(this)
                    .SetUnitSystemGO(ref _snakeGameUnit)
                    .SetElementScale(_elementScaler * 1.25f)
                    .SetElementType(SnakeElementSystem.SnakeElementType.Head);
                // назначаем tag балды змеи
                snake_element.tag = "SnakeHead";
                // добавляем элемент в список звеньев
                _snakeElements.Add(snake_element);
            }
            else
            {
                // создаём элементы звеньев
                link_system
                    .SetElementScale(_elementScaler)
                    .SetElementType(SnakeElementSystem.SnakeElementType.Link);

                // назначаем tag балды змеи
                snake_element.tag = "SnakeLink";
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
        //if (GetElementsCount() > 0)
        //{
        //    Vector3 head_local_position = _snakeElements[0].transform.localPosition;
        //    CURRENT_HEAD_LOCK_POS = head_local_position;

        //    int count = GetElementsCount();
        //    float acceleration = 1 + (count * 0.1f);


        //    for (int i = 0; i < count; ++i)
        //    {
        //        // берем позицию текущего элемента
        //        Vector3 element_local_position = _snakeElements[i].transform.localPosition;
        //        // считаем разницу позиции элемента по оси X
        //        float delta_x = head_local_position.x - element_local_position.x;

        //        if (Mathf.Abs(delta_x) > 0)
        //        {
        //            element_local_position.x += delta_x / (i * 1.5f);

        //            //if (i == 1)
        //            //{
        //            //    element_local_position.x += (delta_x * 0.6f);
        //            //}
        //            //else
        //            //{
        //            //    element_local_position.x += delta_x / i;
        //            //}
        //            //element_local_position.x += delta_x / Mathf.Max(1, ((count - ((count - i) - 1)) / acceleration));
        //        }

        //        _snakeElements[i].transform.localPosition = element_local_position;

        //    }
        //}

        if (GetElementsCount() > 0)
        {
            Vector3 last_local_position = _snakeElements[0].transform.localPosition;
            Vector3 last_link_position = _snakeElements[0].transform.localPosition;

            for (int i = 1; i < GetElementsCount(); ++i)
            {
                // запоминаем предыдущую позицию линка 
                last_link_position = _snakeElements[i].transform.localPosition;
                // берем позицию текущего элемента
                Vector3 element_local_position = last_link_position;
                // назначаем позицию по оси Х по предыдущему элементу
                element_local_position.x = last_local_position.x;
                // возвращаем значение позиции по х от предыдущего элемента
                _snakeElements[i].transform.localPosition = element_local_position;

                // назначаем на прошлую локальную позицию, предыдущую позицию текущего элемента
                last_local_position = last_link_position;
            }
        }
    }
}
