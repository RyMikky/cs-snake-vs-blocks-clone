using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Rendering.FilterWindow;

public class SnakeElementSystem : DissolvableObject
{

    public enum SnakeElementType
    {
        Head, Link, Food
    } 

    public SnakeElementType _elementType = SnakeElementType.Link;
    public ParticleSystem _foodParticle;

    private GameObject _basicGameObject;
    private Transform _basicObjectTransform;                             // базовый трансформ экземпляра
    private SphereCollider _basicTrigger;                                // базовый триггер экземпляра

    private GameObject _baseSnakeSystem;                                 // базовая система обработки змейки
    private SnakeGameUnitSystem _gameUnitSystem;                         // ссылка на класс-конструктор змейки

    private GameObject _baseGameLevelElement;                            // базовый элемент уровня на котором создаётся еда
    private GameLevelElementSystem _gameLevelElementSystem;              // ссылка на класс-конструктор элемента уровня

    private GameLevelSystem _gameLevelSystem;                            // базовая система управления уровнем

    // ------------------------ элементы для работы мышки -----------------

    private Vector3 _lastMousePosition;                                  // поле предыдущей позиции мыши
    public Vector3 lastMousePosition;
    public Vector3 lastHeadPosition;

    public float _objectScaler = 1.0f;

    // ------------------------ элементы для работы с текстовым полем -----

    public GameObject _textObject;
    public TextMeshPro _snakeText;
    public int _snakeLinksCount = 0;

    // ------------------------ элементы для работы звуков ----------------

    private GameSoundSystem _soundSystem;

    // ------------------------ элементы для управления стрелками ---------

    private float _h, _v;                            // значения со стрелок

    public float _maxHorizontalLevel = 1.0f;

    private void Awake()
    {
        _basicGameObject = gameObject;
        _basicObjectTransform = GetComponent<Transform>();
        _basicTrigger = GetComponent<SphereCollider>();
        _soundSystem = FindObjectOfType<GameSoundSystem>();
        _gameLevelSystem = FindObjectOfType<GameLevelSystem>();

        UpdateLinksCount();
    }

    public SnakeElementSystem SetElementScale(float scale)
    {
        Vector3 l_scale = _basicObjectTransform.localScale;
        l_scale.y = scale; l_scale.x = scale; l_scale.z = scale;
        _basicObjectTransform.localScale = l_scale;

        _objectScaler = scale;
        return this;
    }
    public float GetElementScaler() { return _objectScaler; }

    public SnakeElementSystem SetElementType(SnakeElementType type) 
    { 
        _elementType = type;
        return this;
    }
    public SnakeElementType GetElementType() { return _elementType; }

    public SnakeElementSystem SetUnitSystemGO(ref GameObject system) 
    { 
        _baseSnakeSystem = system;
        return this;
    }
    public GameObject GetUnitSystemGO() { return _baseSnakeSystem; }
    public SnakeElementSystem SetGameUnitSystem(SnakeGameUnitSystem system)
    {
        _gameUnitSystem = system;
        return this;
    }
    public SnakeGameUnitSystem GetGameUnitSystem() { return _gameUnitSystem; }

    public SnakeElementSystem SetGameLevelElementSystemGO(ref GameObject system)
    {
        _baseGameLevelElement = system;
        return this;
    }
    public GameObject GetGameLevelElementSystemGO() { return _baseGameLevelElement; }
    public SnakeElementSystem SetGameLevelElementSystem(GameLevelElementSystem system)
    {
        _gameLevelElementSystem = system;
        return this;
    }
    public GameLevelElementSystem GetGameLevelElementSystem() { return _gameLevelElementSystem; }

    public SnakeElementSystem SetLinksCount(int count)
    {
        _snakeLinksCount = count;
        return this;
    }
    public int GetLinksCount() { return _snakeLinksCount; }

    private void FixedUpdate()
    {
        UpdateTriggerRadius();                  // проверяет и обноваляет радиус коллайдера триггера
        UpdateLinksCount();                     // обновляет отображаемое числовое значение на шарике
        KeyBoardController();                   // управление с клавиатуры
        MouseController();                      // управление мышкой
    }

    // проверяет и обноваляет радиус коллайдера триггера
    private void UpdateTriggerRadius()
    {
        if (_basicTrigger != null)
        {
            switch (_elementType)
            {
                case SnakeElementType.Food:
                    if (_basicTrigger.radius != 0.5f) _basicTrigger.radius = 0.5f;
                    break;
                case SnakeElementType.Head:
                case SnakeElementType.Link:
                    if (_basicTrigger.radius != 0.5f) _basicTrigger.radius = 0.5f;
                    break;
            }
        }
    }

    // обновляет отображаемое числовое значение на шарике
    private void UpdateLinksCount()
    {
        if (_textObject != null)
        {
            if (_snakeLinksCount > 0)
            {
                _snakeText.text = _snakeLinksCount.ToString();
                _textObject.SetActive(true);
            }
            else
            {
                _textObject.SetActive(false);
            }
        }
    }


    // обработка столкновений с другими объектами
    private void OnTriggerEnter(Collider other)
    {

        switch (_elementType)
        {
            case SnakeElementType.Food:
                break;
            case SnakeElementType.Link:

                if (other.gameObject.tag == "BoxLTrigger")
                {
                    Vector3 current_transform = gameObject.transform.position;
                    current_transform.x -= 0.5f;
                    gameObject.transform.position = current_transform;
                }

                else if (other.gameObject.tag == "BoxRTrigger")
                {
                    Vector3 current_transform = gameObject.transform.position;
                    current_transform.x += 0.5f;
                    gameObject.transform.position = current_transform;
                }

                break;

            // обработка событий столкновений для головы змейки
            case SnakeElementType.Head:

                if (other.gameObject.tag == "SnakeFood")
                {
                    // если на пути еда - добавляем элемент в змейку
                    _gameUnitSystem.AddSnakeNewLink();
                    // проигрываем вспышку поедания еды
                    _foodParticle.Play();
                    // проигрываем звук "поедания"
                    _soundSystem.PlaySnakeEat();
                    // запускаем удаление "скушанной" еды с элемента уровня
                    other.gameObject
                        .GetComponent<SnakeElementSystem>()
                        .DestroyFoodElement();
                }

                else if (other.gameObject.tag == "BoxFTrigger")
                {
                    // если на пути появился ящик
                    other.gameObject
                        .GetComponentInParent<ElementBoxSystem>()
                        .DecrementBoxScore();
                    // проигрываем звук "столкновения"
                    _soundSystem.PlayBoxBreak();
                    // удаляем звено из змейки
                    _gameUnitSystem.RemoveLastSnakeElements(0.2f);

                    // двигаем весь уровень назад
                    _gameLevelSystem.MovingLevelDown(4.5f);
                }

                else if(other.gameObject.tag == "BoxLTrigger")
                {
                    Vector3 current_transform = gameObject.transform.position;
                    current_transform.x -= 0.5f;
                    gameObject.transform.position = current_transform;
                }

                else if (other.gameObject.tag == "BoxRTrigger")
                {
                    Vector3 current_transform = gameObject.transform.position;
                    current_transform.x += 0.5f;
                    gameObject.transform.position = current_transform;
                }

                break; 
            
        }
        
    }

    private void DestroyFoodElement()
    {
        if (_elementType == SnakeElementType.Food)
        {
            // удаляемся из элемента уровня на котором существуем
            _gameLevelElementSystem.DestroyFoodElement(0.2f, ref _basicGameObject);
        }
    }

    [ExecuteInEditMode]
    private void MouseController()
    {
        // управление работает только для головы
        if (_elementType == SnakeElementType.Head)
        {
            // при нажатой кнопке мышки
            if (Input.GetMouseButton(0))
            {

                lastMousePosition = Input.mousePosition - _lastMousePosition;
                // берем компоненту мышки по Х
                float horizontal = lastMousePosition.x;

                Vector3 mouse_from_camera_pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
                lastMousePosition = mouse_from_camera_pos;
                Vector3 current_transform = gameObject.transform.position;
                current_transform.x = mouse_from_camera_pos.x;
                transform.position = current_transform;


                RaycastHit hit;  // стреляем кастом
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    // если кастуется на "голову" змейки
                    if(true /*hit.collider.gameObject.tag == "SnakeHead"*/)
                    {
                        
                    }
                }
            }

            lastHeadPosition = gameObject.transform.position;
            //lastMousePosition = Input.mousePosition;
            _lastMousePosition = Input.mousePosition;
        }
    }

    // получение данных по горизонтали и вертикали со стрелок
    void Inputs()
    {
        _h = Input.GetAxis("Horizontal"); _v = Input.GetAxis("Vertical");
    }

    [ExecuteInEditMode]
    private void KeyBoardController()
    {
        // управление работает только для головы
        if (_elementType == SnakeElementType.Head)
        {
            Inputs();   // голова постоянно будет получать данные со стрелок

            Vector3 current_transform = gameObject.transform.position;
            current_transform.x -= _h;

            if (Mathf.Abs(current_transform.x) > Mathf.Abs(_maxHorizontalLevel)) 
            { 
                if (_h < 0)
                {
                    current_transform.x = _maxHorizontalLevel;
                }
                else
                {
                    current_transform.x = -_maxHorizontalLevel;
                }
            }

            gameObject.transform.position = current_transform;

            if (_v > 0)
            {
                _gameLevelSystem.IncreaseLevelSpeed(_v * 0.01f);
            }
            else
            {
                _gameLevelSystem.DecreaseLevelSpeed(_v * 0.01f);
            }
        }
    }
}
