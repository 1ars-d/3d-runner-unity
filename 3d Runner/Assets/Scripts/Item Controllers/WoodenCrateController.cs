using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodenCrateController : MonoBehaviour
{
    [SerializeField] private GameObject _createWhole;
    [SerializeField] private GameObject _createDesctructed;
    [SerializeField] private GameObject _scoreText;
    [SerializeField] private AudioClip _breakSound;

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
        StartCoroutine(_camController.Shake(0.13f, 0.08f));
        SoundManager.Instance.PlaySound(_breakSound);
        _createWhole.SetActive(false);
        _createDesctructed.SetActive(true);
        _scoreText.SetActive(true);
    }
}
