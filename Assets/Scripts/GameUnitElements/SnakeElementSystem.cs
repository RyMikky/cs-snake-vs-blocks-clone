using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeElementSystem : MonoBehaviour
{
    public enum SnakeElementType
    {
        Head, Link
    } 

    public SnakeElementType _elementType = SnakeElementType.Link;

    private Transform _basicObject;
    public float _objectScaler = 1.0f;

    private void Awake()
    {
        _basicObject = GetComponent<Transform>();
    }

    public SnakeElementSystem SetElementScale(float scale)
    {
        Vector3 l_scale = _basicObject.localScale;
        l_scale.y = scale; l_scale.x = scale; l_scale.z = scale;
        _basicObject.localScale = l_scale;

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
}
