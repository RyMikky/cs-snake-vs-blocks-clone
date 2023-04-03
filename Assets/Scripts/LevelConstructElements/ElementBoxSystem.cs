using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameConstantsKeeper;

public class ElementBoxSystem : MonoBehaviour
{
    public enum BoxConstructMode
    {
        onColorSwitch, onBoxScore
    }

    public BoxConstructMode _boxConstructMode;
    public GameConstantsKeeper.BoxColorPalette _colorPalette;
    public int _boxScore;
    private int _lastScore;
    public ElementBoxSystem SetBoxScore(int score) { _boxScore = score; return this; }
    public int GetBoxScore() { return _boxScore; }

    public float _boxScaler;
    private float _lastScaler;
    public ElementBoxSystem SetBoxScaler(float scaler) { _boxScaler = scaler; return this; }
    public float GetBoxScaler() { return _boxScaler; }

    public Shader _boxShader;
    public TextMeshPro _boxText;

    private GameConstantsKeeper _gameKeeper;
    private Transform _boxTransform;
    private MeshRenderer _boxMeshRenderer;
    private GameConstantsKeeper.ColorPallete _boxPalette;

    [Header("Деббаговый блок")]
    public Color INC_MAIN_COLOR;
    public Color INC_PULSE_COLOR;
    public Color INC_FRENS_COLOR;

    private void Awake()
    {
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
        if (_lastScore != _boxScore)
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
        _lastScore = _boxScore;

        if (_lastScore < 0)
        {
            _boxText.text = "0";
        }
        else
        {
            _boxText.text = _lastScore.ToString();
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
                _boxPalette = _gameKeeper.GetColorPaletteByScore(_lastScore);
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
        if (_lastScore > 1)
        {
            // пока мультипликатор ящика больше одного
            _boxScore--;        // декреммируем номер и вызываем переопределение цвета
        }
        else
        {
            DestroyTheBox();
        }
    }

    public void DestroyTheBox()
    {

    }

}
