using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableCulling : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //GameObject _camera = GameObject.FindGameObjectWithTag("MainCamera");
        //Camera cam = _camera.GetComponent<Camera>();
        //// boundsTarget is the center of the camera's frustum, in world coordinates:
        //Vector3 camPosition = _camera.transform.position;
        //Vector3 normCamForward = Vector3.Normalize(_camera.transform.forward);
        //float boundsDistance = (cam.farClipPlane - cam.nearClipPlane) / 2 + cam.nearClipPlane;
        //Vector3 boundsTarget = camPosition + (normCamForward * boundsDistance);

        //// The game object's transform will be applied to the mesh's bounds for frustum culling checking.
        //// We need to "undo" this transform by making the boundsTarget relative to the game object's transform:
        //Vector3 realtiveBoundsTarget = this.transform.InverseTransformPoint(boundsTarget);

        //// Set the bounds of the mesh to be a 1x1x1 cube (actually doesn't matter what the size is)
        //MeshRenderer mesh = GetComponent<MeshRenderer>();
        //mesh.bounds = new Bounds(realtiveBoundsTarget, Vector3.one);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
