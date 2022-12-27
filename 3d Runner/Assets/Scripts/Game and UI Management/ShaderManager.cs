using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderManager : MonoBehaviour
{
    [Header("Curve Shader")]
    [SerializeField] private float _curveY;
    [SerializeField] private float _curveX;

    private void Awake()
    {
        Shader.SetGlobalFloat("_curveX", _curveX);
        Shader.SetGlobalFloat("_curveY", _curveY);
    }

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalFloat("_curveX", _curveX);
        Shader.SetGlobalFloat("_curveY", _curveY);
    }
}
