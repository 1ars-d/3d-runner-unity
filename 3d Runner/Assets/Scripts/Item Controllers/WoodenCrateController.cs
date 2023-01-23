using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodenCrateController : MonoBehaviour
{
    [SerializeField] private GameObject _createWhole;
    [SerializeField] private GameObject _createDesctructed;

    private CameraController _camController;


    // Start is called before the first frame update
    void Start()
    {
        _camController = GameObject.FindGameObjectWithTag("VCam").GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyCrate()
    {
        StartCoroutine(_camController.Shake(0.1f, 0.07f));
        _createWhole.SetActive(false);
        _createDesctructed.SetActive(true);
    }
}
