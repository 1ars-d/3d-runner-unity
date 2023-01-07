using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    Camera _camera;

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        _camera.cullingMatrix = Matrix4x4.Ortho(-99999, 99999, -99999, 99999, 0.001f, 99999) *
                             Matrix4x4.Translate(Vector3.forward * -99999 / 2f) *
                             _camera.worldToCameraMatrix;
    }

    //public void OnPreCull()
    //{
    //    _camera.cullingMatrix = Matrix4x4.Ortho(-99999, 99999, -99999, 99999, 0.001f, 99999) *
    //                         Matrix4x4.Translate(Vector3.forward * -99999 / 2f) *
    //                         _camera.worldToCameraMatrix;
    //}
}
