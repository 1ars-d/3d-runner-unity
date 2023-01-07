using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderManager : MonoBehaviour
{
    [Header("Curve Shader")]
    [SerializeField] private float _curveY;
    [SerializeField] private float _curveX;
    [SerializeField] private bool _active;

    private void Awake()
    {
        if (_active)
        {
            Shader.SetGlobalFloat("_curveX", _curveX);
            Shader.SetGlobalFloat("_curveY", _curveY);
        } else
        {
            Shader.SetGlobalFloat("_curveX", 0);
            Shader.SetGlobalFloat("_curveY", 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_active)
        {
            Shader.SetGlobalFloat("_curveX", _curveX);
            Shader.SetGlobalFloat("_curveY", _curveY);
        }
    }
}
