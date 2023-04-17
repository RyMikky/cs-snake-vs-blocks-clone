using System.Collections.Generic;
using UnityEngine;

public class SnakeGameUnitSystem : MonoBehaviour
{
    private GameKeeper _mainGameKeeper;         // базовая система управления игрой и игровыми состояниями
    public SnakeGameUnitSystem SetGameKeeper(GameKeeper keeper) { _mainGameKeeper = keeper; return this; }

    private GameUISystem _gameUISystem;         // базовый UI-интерфейс, нужен, чтобы отображать количество экстра-жизней и очков
    public SnakeGameUnitSystem SetGameUISystem(GameUISystem uISystem) { _gameUISystem = uISystem; return this; }

    // TODO Сюда необходимо добавить систему отображения очков и экстра жизней

    public enum SnakeUnitMode
    {
        Empty, Demo, Default
    }

    public SnakeUnitMode _snakeUnitMode = SnakeUnitMode.Default;

    public GameObject _snakeElementPrefab;                                           // базовый префаб звена змейки

    public List<GameObject> _snakeElements = new List<GameObject>();                 // список звеньев с которым работает система
     
    public float _elementScaler = 1.0f;                                              // скаляр для изменения масштабов
    public SnakeGameUnitSystem SetElementScaler(float scaler) { _elementScaler = scaler; return this; }
    public int _maxVisibleLinks = 6;                                                 // максимальное количество отображаемых звеньев
    public SnakeGameUnitSystem SetMaxVisibleLinks(int links) { _maxVisibleLinks = links; return this; }
    private int _invisibleLinks = 0;                                                 // внутренний счётчик невидимых "виртуальных" звеньев

    public int _snakeExtralife = 3;                                                 // количество экстра жизней
    public SnakeGameUnitSystem SetSnakeExtraLife(int life) 
    { 
        _snakeExtralife = life;

        if (_gameUISystem == null)
        {
            _gameUISystem = FindObjectOfType<GameUISystem>();
        }

        if (_gameUISystem != null)
        {
            _gameUISystem.SetExtraLifeCount(life);
        }

        return this; 
    }
    public int GetSnakeExtraLife() { return _snakeExtralife; }

    private int _snakeScoreScaler = 50;                                              // множитель очков за каждое "съеденное" звено
    public SnakeGameUnitSystem SetSnakeScoreScaler(int scaler) { _snakeScoreScaler = scaler; return this; }
    public int GetSnakeScoreScaler() { return _snakeScoreScaler; }

    private GameObject _snakeHeadGO;                                                 // объект головы текущей змейки
    private SnakeElementSystem _snakeHeadSystem;                                     // скрипт головы змейки

    private GameObject _snakeGameUnit;                                               // текущий объект скрипта
    private System.Random _random = new System.Random();

    public int _currentScore = 0;                                                    // счётчик очков на змейке
    public int GetSnakeCurrentScore() { return _currentScore; }
    public SnakeGameUnitSystem SetSnakeCurrentScore(int score) 
    {
        _currentScore = score; 

        if (_gameUISystem == null)
        {
            _gameUISystem = FindObjectOfType<GameUISystem>();
        }

        if (_gameUISystem != null)
        {
            _gameUISystem.SetTextScore(score.ToString());
        }
        
        return this; 
    }
    public SnakeGameUnitSystem ResetSnakeCurrentScore() 
    { 
        _currentScore = 0;

        if (_gameUISystem == null)
        {
            _gameUISystem = FindObjectOfType<GameUISystem>();
        }

        if (_gameUISystem != null)
        {
            _gameUISystem.SetTextScore(_currentScore.ToString());
        }

        return this; 
    }

    private GameConstantsKeeper.GameLevelConfig _currentConfig;                      // сохраненная копия конфигурации

    // флаг включения триггеров у элементов, по дефолту выключен, соответственно коллизии обрабатываться не будут
    private bool _currentTriggerEnable = false;
    private bool _currentKeyboardEnable = true;                                      // флаг включения управления с клавиатуры
    private bool _currentMouseEnable = true;                                         // флаг включения управления с мышки

    // установка конфигурации c флагом сброса экстра жизней
    public SnakeGameUnitSystem SetSnakeConfiguration(GameConstantsKeeper.GameLevelConfig config, bool reset_extra_life)
    {
        _currentConfig = config;
        _maxVisibleLinks = config._snakeLinkVisible;
        _snakeScoreScaler = config._snakeScoreScaler;

        // если передан флаг сброса, то записывается значение из конфига,
        // иначе останется текущим на момент записи параметров
        if (reset_extra_life)
        {
            _snakeExtralife = config._snakeExtraLife;
        }

        _gameUISystem.SetExtraLifeCount(_snakeExtralife);             // обновляем количество экстра-жизней

        return this;
    }
    // удаляем все элементы змеи
    public SnakeGameUnitSystem DestroySnakeUnit()
    {
        _snakeUnitMode = SnakeUnitMode.Empty;           // устанавливаем в пустой режим

        for(int i = 0; i < _snakeElements.Count; i++)
        {
            Destroy(_snakeElements[i]);
        }
        _snakeElements.Clear();                         // очищаем элементы в списке
        _snakeHeadGO = null;                            // сбрасываем голову
        _snakeHeadSystem = null;                        // сбрасываем систему головы

        return this;
    }
    // реконструирует новую змейку по переданным параметрам и флагам сброса экстра жизней и очков, с флагом активации триггеров, контроллеров и в общем замейки
    public SnakeGameUnitSystem ConstructNewSnake(GameConstantsKeeper.GameLevelConfig config, 
        bool reset_extra_life, bool reset_score, bool trigger_enable, bool keyboard_enable, bool mouse_enable, bool active)
    {
        SetSnakeConfiguration(config, reset_extra_life).ConstructNewSnake(reset_score, trigger_enable, keyboard_enable, mouse_enable, active);
        return this;
    }
    // конструирует новую змейку используя уже установленные параметры с флагами сброса очков и активации триггеров, контроллеров и в общем замейки
    public SnakeGameUnitSystem ConstructNewSnake(bool reset_score, bool trigger_enable, bool keyboard_enable, bool mouse_enable, bool active) 
    {
        DestroySnakeUnit();                                                             // удаляем все элементы

        if (reset_score) SetSnakeCurrentScore(0);                                       // сбрасываем в ноль очки

        ConstructNewSnake(trigger_enable, keyboard_enable, mouse_enable, active);       // запускаем конструктор
        return this; 
    }
    private SnakeGameUnitSystem ConstructNewSnake(bool trigger_enable, bool keyboard_enable, bool mouse_enable, bool active)
    {
        _currentTriggerEnable = trigger_enable;
        _currentKeyboardEnable = keyboard_enable;
        _currentMouseEnable = mouse_enable;

        if (_gameUISystem == null) _gameUISystem = FindObjectOfType<GameUISystem>();

        // если у нас есть модуль UI
        if (_gameUISystem != null)
        {   
            // передаем ему данные о количестве экстра жизней и очках
            _gameUISystem
                .SetExtraLifeCount(_snakeExtralife)
                .SetTextScore(_currentScore.ToString());

            //if (_snakeExtralife == 0)
            //{
            //    Debug.Log("Передаю ноль экстра жизней");
            //}
        }
        else
        {
            Debug.Log("Нет UI");
        }

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
                    .SetGameKeeper(_mainGameKeeper)
                    .SetTriggerEnable(_currentTriggerEnable)
                    .SetKeyboardController(_currentKeyboardEnable)
                    .SetMouseController(_currentMouseEnable)
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
                    .SetTriggerEnable(_currentTriggerEnable)
                    .SetKeyboardController(false)       // для звеньев обработка ввода с клавиатуры не требуется
                    .SetMouseController(false)          // для звеньев обработка ввода с мышки не требуется
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

        _snakeGameUnit.SetActive(active);

        return this;
    }

    private void Awake()
    {
        _snakeGameUnit = gameObject;

        switch (_snakeUnitMode)
        {
            case SnakeUnitMode.Empty:
                break;

            case SnakeUnitMode.Demo:
                // конструирует змейку с базовыми параметрами, нулевыми очками
                // выключенными триггерами, включенным управлением с клавиатуры и выключенной мышкой
                ConstructNewSnake(true, false, true, false, true);
                break;

            case SnakeUnitMode.Default:
                // конструирует змейку с базовыми параметрами, нулевыми очками
                // включенными триггерами, включенным управлением с клавиатуры и выключенной мышкой
                ConstructNewSnake(true, true, true, false, true);
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
        if (_snakeElements.Count + _invisibleLinks> 0)
        {
            _snakeHeadSystem.SetLinksCount(_snakeElements.Count + _invisibleLinks);
        }
    }

    // устанавливает включение и отключение триггера в элементах змеи
    public SnakeGameUnitSystem SetElementsTriggerEnable(bool enable)
    {
        foreach (var item in _snakeElements) 
        {
            item.GetComponent<SnakeElementSystem>().SetTriggerEnable(enable);
        }
        return this;
    }

    // добавляет новый элемент в змею
    public SnakeGameUnitSystem AddSnakeNewLink()
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
            // если суммарное количество элементов 99 и следующее будет 100
            if (_snakeElements.Count + _invisibleLinks == 99)
            {
                _invisibleLinks = 0;                                          // обнуляем "виртуальные" звенья
                _snakeExtralife++;                                            // добавляем экстра жизнь
                _gameUISystem.SetExtraLifeCount(_snakeExtralife);             // обновляем количество экстра-жизней
            }
            else
            {
                _invisibleLinks++;            // иначе просто инкремируем "виртуальное" звено
            }
        }

        // обновляем очки за "поедание" - просто прибавляем скалер очков
        _currentScore += _snakeScoreScaler;
        
        if (_gameUISystem != null)
        {
            // загружаем очки в соответствующее UI-поле
            _gameUISystem.SetTextScore(_currentScore.ToString());
        }
        

        return this;
    }
    // удаляет крайний элемент змеи
    public SnakeGameUnitSystem RemoveLastSnakeElements(float time)
    {
        // если количество всех возможных элементов змейки как видимых так и невидимых больше одного
        if ( _snakeElements.Count + _invisibleLinks > 1)
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
        // иначе, если у нас крайнее звено, необходимо использовать экстра-жизнь
        else
        {
            // если имеются экстра-жизни
            if (_snakeExtralife > 0)
            {
                _snakeExtralife--;                                                        // отнимаем экстра жизнь
                _gameUISystem.SetExtraLifeCount(_snakeExtralife);            // обновляем количество экстра-жизней
                // реконструируем новую змейку с сохранением очков и текущими настройками триггеров и контроллеров
                ConstructNewSnake(false, _currentTriggerEnable, _currentKeyboardEnable, _currentMouseEnable, true);
            }
            else
            {
                if (_mainGameKeeper != null)
                {
                    _gameUISystem.SetAllExtraLifeInactive();                 // закрываем все экстра-жизни
                    _mainGameKeeper.GameOver(_currentScore);             // передаем киперу набранные очки
                    _currentScore = 0;                                   // сбрасываем набранные очки
                    DestroySnakeUnit();                                  // рессетим змейку
                }
            }
        }

        return this;
    }

    // транслирует киперу количество очков и экстра-жизней
    public void LevelComplette()
    {
        _currentScore += (_snakeExtralife * _snakeScoreScaler);           // прибавляем очков на произведение количество экстра жизней на множитель очков
        _gameUISystem.SetTextScore(_currentScore.ToString());             // загружаем очки в соответствующее UI-поле
        _mainGameKeeper.LevelComplette(_currentScore, _snakeExtralife);   // передаем управление киперу
    }

    // возвращает количество элементов в змее
    public int GetElementsCount()
    {
        return _snakeElements.Count;
    }

    // изменение масштаба элементов змейки
    public SnakeGameUnitSystem SetElementsScale(float scale)
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

        return this;
    }

    // УСТАРЕЛО конструктор тестовой змейки 
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