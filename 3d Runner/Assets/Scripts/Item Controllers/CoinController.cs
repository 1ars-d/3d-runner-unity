using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    private Animator _animator;
    [Range(0, 100)]
    [SerializeField] private float _maxOffset = 0.4f;
    [SerializeField] private GameObject _collectEffect;

    private GameObject _player;
    private PlayerController _playerController;
    private bool _movingToPlayer;

    private GameManager _gameManager;
    private PowerUpManager _PUManager;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _PUManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PowerUpManager>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerController = _player.GetComponent<PlayerController>();
        StartCoroutine(PlayRotateAnimation(Random.Range(0f, _maxOffset)));
    }

    IEnumerator PlayRotateAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        _animator.Play("Rotate");
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameManager.IsRunning && _PUManager.MagnetIsActive && !_movingToPlayer && Vector3.Distance(transform.position, _player.transform.position) <= 8)
        {
            _movingToPlayer = true;
            _animator.enabled = false;
            StartCoroutine(MoveToPlayer());
        }
    }

    private IEnumerator MoveToPlayer()
    {
        float timeElapsed = 0;
        while (timeElapsed < 2)
        {
            float t = timeElapsed / 2;
            t = t * t * (3f - 2f * t);
            float playerY = _playerController._playerCollider.center.y;
            transform.position = Vector3.Lerp(transform.position, new Vector3(_player.transform.position.x, _player.transform.position.y + playerY, _player.transform.position.z + 0.5f), t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _gameManager.AddCoin();
            float yPos = other.gameObject.transform.position.y + .9f;
            if (other.gameObject.GetComponent<PlayerController>()._inRoll)
            {
                yPos -= .5f;
            }
            Instantiate(_collectEffect, new Vector3(other.gameObject.transform.position.x, yPos, other.gameObject.transform.position.z + .15f), Quaternion.Euler(new Vector3(-90 + Random.Range(-90, 90), 90, 90)), other.gameObject.transform);
            Destroy(gameObject);
        }
    }
}
