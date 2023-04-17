using TMPro;
using UnityEngine;
//using static GameConstantsKeeper;

public class ElementBoxSystem : DissolvableObject
{
    public enum BoxConstructMode
    {
        onColorSwitch, onBoxScore
    }

    public BoxConstructMode _boxConstructMode;
    public GameConstantsKeeper.BoxColorPalette _colorPalette;
    public int _boxValue;
    private int _lastValue;
    public ElementBoxSystem SetBoxValue(int value) { _boxValue = value; return this; }
    public int GetBoxValue() { return _boxValue; }
    public bool _showBoxValue = true;

    public float _boxScaler;
    private float _lastScaler;
    public ElementBoxSystem SetBoxScaler(float scaler) { _boxScaler = scaler; return this; }
    public float GetBoxScaler() { return _boxScaler; }

    public Shader _boxShader;
    public GameObject _textObject;
    public TextMeshPro _boxText;
    public ParticleSystem _boxParticle;
    public GameObject _boxTriggers;

    private GameConstantsKeeper _gameKeeper;
    private Transform _boxTransform;
    private MeshRenderer _boxMeshRenderer;
    private GameConstantsKeeper.ColorPallete _boxPalette;
    private GameObject _basicGameObject;

    [Header("Деббаговый блок")]
    public Color INC_MAIN_COLOR;
    public Color INC_PULSE_COLOR;
    public Color INC_FRENS_COLOR;

    private void Awake()
    {
        _basicGameObject = gameObject;
        _gameKeeper = FindObjectOfType<GameConstantsKeeper>();
        _boxMeshRenderer = GetComponent<MeshRenderer>();
        _boxTransform = GetComponent<Transform>();
        _boxPalette = new GameConstantsKeeper.ColorPallete();

        UpdateBoxScore();             // пересчет внутреннего значения множителя очков
        UpdateBoxPalette();           // обновление цветовой палитры блока

        UpdateBoxScaler();            // пересчет внутреннего значения масштаба блока
        UpdateBoxScale();             // обновление отображаемого масштаба элемента
    }

    private void FixedUpdate()
    {
        if (_lastValue != _boxValue)
        {
            UpdateBoxScore();         // пересчет внутреннего значения множителя очков
            UpdateBoxPalette();       // обновление цветовой палитры блока
        }

        if (_lastScaler != _boxScaler)
        {
            UpdateBoxScaler();        // пересчет внутреннего значения масштаба блока
            UpdateBoxScale();         // обновление отображаемого масштаба элемента
        }
    }

    /// <summary>
    /// Метод присваивает текущее заданное значение количества очков внутреннему полю класса.
    /// Обновляет вывод количества очков в TextMeshPro основного блока.
    /// </summary>
    void UpdateBoxScore()
    {
        if (_showBoxValue)
        {
            _lastValue = _boxValue;

            if (_lastValue < 0)
            {
                _boxText.text = "0";
            }
            else
            {
                _boxText.text = _lastValue.ToString();
            }
        }  
    }
    /// <summary>
    /// Запарашивает цветовую палетку в классе-хранителе констант.
    /// Обновляет цвета в рендере объекта, перезаписывает дебаговые поля.
    /// </summary>
    void UpdateBoxPalette()
    {
        switch (_boxConstructMode)
        {
            // в зависимости от типа работы ящика берем палетку соответствующей функцией
            case BoxConstructMode.onColorSwitch:
                _boxPalette = _gameKeeper.GetColorPaletteByType(_colorPalette);
                break;
            case BoxConstructMode.onBoxScore:
                _boxPalette = _gameKeeper.GetColorPaletteByScore(_lastValue);
                break;
        }

        // обновляем значения в сообственном рендере ящика
        _boxMeshRenderer.material.SetColor(_boxShader.GetPropertyName(0), _boxPalette.MainColor);
        _boxMeshRenderer.material.SetColor(_boxShader.GetPropertyName(1), _boxPalette.PulseColor);
        _boxMeshRenderer.material.SetColor(_boxShader.GetPropertyName(4), _boxPalette.FresnelColor);

        // обновляем дебаговую позицию
        INC_MAIN_COLOR = _boxPalette.MainColor;
        INC_PULSE_COLOR = _boxPalette.PulseColor;
        INC_FRENS_COLOR = _boxPalette.FresnelColor;
    }
    /// <summary>
    /// Обновляет текущий множитель масштаба блока внутреннего поля.
    /// </summary>
    void UpdateBoxScaler()
    {
        _lastScaler = _boxScaler;
    }
    /// <summary>
    /// Обновляет текущий множитель масштаба блока у непосредственного локального трансформа.
    /// </summary>
    void UpdateBoxScale()
    {
        _boxTransform.localScale = new Vector3(_lastScaler, _lastScaler, _lastScaler);
    }
    /// <summary>
    /// Декреммирует величину очков блока, если их не менее едмницы, в противном случае вызывает удаление блока.
    /// </summary>
    public void DecrementBoxScore()
    {
        if (_lastValue > 1)
        {
            // пока мультипликатор ящика больше одного
            _boxValue--;        // декреммируем номер и вызываем переопределение цвета
        }
        else
        {
            // выключаем все триггеры чтобы не было коллизий
            _boxTriggers.SetActive(false);
            DestroyTheBox();    // запускаем удаление ящика
        }
    }

    public void DestroyTheBox()
    {
        // делаем "взрыв"
        _boxParticle.Play();
        // проигрываем звук разламывания ящика
        FindObjectOfType<GameSoundSystem>().PlayBoxBreak();
        // запрашиваем у родителя растворение и удаление текущего ящика
        GetComponentInParent<GameLevelElementSystem>()
            .DestroyBoxElement(0.3f, ref _basicGameObject);
    }

    public void PlayBoxParticle()
    {
        _boxParticle.Play();
    }

    public ElementBoxSystem ShowBoxValue(bool show)
    {
        _showBoxValue = show;
        return this;
    }

}
