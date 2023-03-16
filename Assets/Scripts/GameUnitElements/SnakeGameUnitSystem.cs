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

    // ��������� ����� ������� � ����
    public void AddSnakeElement(ref GameObject snakeElement)
    {
        _snakeElements.Add(snakeElement);
    }

    // ������� ������� ������� ����
    public void RemoveLastSnakeElements()
    {
        if (GetElementsCount() != 0)
        {
            _snakeElements.RemoveAt(GetElementsCount() - 1);
        }
    }

    // ��������� ���������� ��������� � ����
    public int GetElementsCount()
    {
        return _snakeElements.Count;
    }

    public void SetElementsScale(float scale)
    {
        if (scale != _elementScaler)
        {
            _elementScaler = scale; // ��������� ����� �������

            for (int i = 0; i < GetElementsCount(); ++i)
            {
                // ��������� ������� �������� ����
                var system = _snakeElements[i].GetComponent<SnakeElementSystem>();
                // ������������� ������
                system.SetElementScale(_elementScaler);
            }
        }
    }

    // ����������� �������� ������
    private void TestSnakeConstruct()
    {
        for (int i = 0; i < 17; ++i)
        {
            // ������ ������ �������� ���� � ������� ����������
            GameObject snake_element = Instantiate(_snakeElementPrefab, transform) as GameObject;

            if (i == 0)
            {
                // ������ ������� �������� � ������� ��������� ����� � �� 25% ������ ���������
                snake_element.GetComponent<SnakeElementSystem>()
                    .SetElementScale(_elementScaler * 1.25f)
                    .SetElementType(SnakeElementSystem.SnakeElementType.Head);
                // ��������� ������� � ������ �������
                _snakeElements.Add(snake_element);
            }
            else
            {
                // ������ �������� �������
                snake_element.GetComponent<SnakeElementSystem>()
                    .SetElementScale(_elementScaler)
                    .SetElementType(SnakeElementSystem.SnakeElementType.Link);

                // ��� ����������� ���������� ��������� n-��� �������� ����� ��������� ��������� �����������
                Vector3 element_position = _snakeElements[i - 1].transform.localPosition;

                if (i == 1)
                {
                    // ���� ������� ������ ������ ����� ������, �� �������� ������� � ��������� �� ������ ���������� �� 112,5%
                    element_position.z += (_snakeElements[i - 1].GetComponent<SnakeElementSystem>().GetElementScaler() * 1.125f);
                }
                else
                {
                    // ��� ����������� ��������, ������� � ��������� �� ������ ���������� �� 125%
                    element_position.z += (_snakeElements[i - 1].GetComponent<SnakeElementSystem>().GetElementScaler() * 1.25f);
                }

                // ����� ��������� ������� �������� � ����
                snake_element.transform.localPosition = element_position;
                // ��������� ������� � ������ �������
                _snakeElements.Add(snake_element);
            }
        }
    }

    // ���������� ������� ��������� ������ �� ��������� ��������
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
                // ����� ������� �������� ��������
                Vector3 element_local_position = _snakeElements[i].transform.localPosition;
                // ������� ������� ������� �������� �� ��� X
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
