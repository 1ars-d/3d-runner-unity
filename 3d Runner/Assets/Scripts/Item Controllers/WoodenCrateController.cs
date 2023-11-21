using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WoodenCrateController : MonoBehaviour
{
    [SerializeField] private GameObject _createWhole;
    [SerializeField] private GameObject _createDesctructed;
    [SerializeField] private TextMeshPro _scoreText;
    [SerializeField] private AudioClip _breakSound;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private int _coinValue;
    [SerializeField] private AudioClip _coinSoundEffect;
    [SerializeField] private bool _explode = false;
    [SerializeField] private AudioClip _explosionSoundEffect;

    private CameraController _camController;


    // Start is called before the first frame update
    void Start()
    {
        _camController = GameObject.FindGameObjectWithTag("VCam").GetComponent<CameraController>();
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyCrate()
    {
        StartCoroutine(_camController.Shake(0.13f, 0.08f));
        SoundManager.Instance.PlaySound(_breakSound);
        if (_coinValue > 0)
        {
            SoundManager.Instance.PlaySound(_coinSoundEffect);
            _scoreText.SetText("+" + _coinValue.ToString());
            _gameManager.coins += _coinValue;
        }
        if (_explode)
        {
            SoundManager.Instance.PlaySound(_explosionSoundEffect);
            _gameManager.OnExplosion();
        }
        _gameManager.gameScore += 100;
        _createWhole.SetActive(false);
        _createDesctructed.SetActive(true);
        _scoreText.gameObject.SetActive(true);
    }
}
