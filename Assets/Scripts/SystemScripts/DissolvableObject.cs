using TMPro;
using UnityEngine;

public class DissolvableObject : MonoBehaviour
{
    public float dissolveDuration = 1f; // длительность растворения в секундах
    protected Renderer _renderer;
    protected TextMeshPro _textMeshPro;
    protected bool _isDissolving = false;
    protected float _dissolveTimer = 0f;

    public float solveDuration = 1f; // длительность появления в секундах
    protected bool _isSolving = false;
    protected float _solveTimer = 0f;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();

        if (gameObject.tag == "GameBox")
        {
            _textMeshPro = GetComponentInChildren<TextMeshPro>();
        }
    }

    private void TryGetRenderer()
    {
        _renderer = GetComponent<Renderer>();
    }

    public DissolvableObject StartSolving()
    {
        _isSolving = true;
        _solveTimer = 0f;
        return this;
    }

    public DissolvableObject StartSolving(float time)
    {
        _isSolving = true;
        _solveTimer = 0f;
        solveDuration = time;
        return this;
    }

    public DissolvableObject StartDissolving()
    {
        _isDissolving = true;
        _dissolveTimer = 0f;
        return this;
    }

    public DissolvableObject StartDissolving(float time)
    {
        _isDissolving = true;
        _dissolveTimer = 0f;
        dissolveDuration = time;
        return this;
    }

    // используется для изменения цвета линии финиша
    public DissolvableObject SetMainColor(Color color)
    {
        if (_renderer == null)
        {
            TryGetRenderer();
        }

        if(_renderer != null)
        {
            _renderer.material.SetColor("_BaseColor", color);
        }
        else
        {
            Debug.Log("Нет рендерера");
        }
        
        return this;
    }

    protected virtual void Update()
    {
        if (_isDissolving)
        {
            if (_renderer == null)
            {
                TryGetRenderer();
            }

            // Увеличиваем таймер на время, прошедшее с последнего кадра
            _dissolveTimer += Time.deltaTime;

            // Вычисляем значение параметра "прозрачность" на основе текущего времени
            float param = Mathf.Clamp01(_dissolveTimer / dissolveDuration);

            if (_renderer != null)
            {
                // Устанавливаем значение параметра "прозрачность" в настройки рендера
                _renderer.material.SetFloat("_NoiseStep", 1 - param);
            }

            if (_textMeshPro != null)
            {
                _textMeshPro.alpha = 1 - param;
            }
            
            // Если объект полностью растворился, удаляем его
            if (param == 1f)
            {
                Destroy(gameObject);
            }
        }

        if (_isSolving)
        {
            if (_renderer == null)
            {
                TryGetRenderer();
            }

            // Увеличиваем таймер на время, прошедшее с последнего кадра
            _solveTimer += Time.deltaTime;

            // Вычисляем значение параметра "прозрачность" на основе текущего времени
            float param = Mathf.Clamp01(_solveTimer / solveDuration);
            
            // необходимо проверять наличие, так как может оказаться так,
            // что на одну миллисекунду скрипт будет быстрее создания компонентов
            // и всё вылетит с ошибкой NullReferenceException
            if (_renderer != null)
            {
                // Устанавливаем значение параметра "прозрачность" в настройки рендера
                _renderer.material.SetFloat("_NoiseStep", param);
            }

            if (_textMeshPro != null)
            {
                _textMeshPro.alpha = param;
            }

            // Если объект появился, то восстанавливаем его
            if (param == 1f)
            {
                _isSolving = false;
            }
        }
    }
}
