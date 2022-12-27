using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxController : MonoBehaviour
{
    [Header("Skybox")]
    [SerializeField] private Material _skybox;
    [SerializeField] private float _skyBoxRotationIncrease;

    // Update is called once per frame
    void Update()
    {
        float _currentSkyBoxRotation = _skybox.GetFloat("_Rotation");
        if (_currentSkyBoxRotation < 360)
        {
            _skybox.SetFloat("_Rotation", _currentSkyBoxRotation + _skyBoxRotationIncrease);
        }
        else
        {
            _skybox.SetFloat("_Rotation", 0);
        }
    }

}
