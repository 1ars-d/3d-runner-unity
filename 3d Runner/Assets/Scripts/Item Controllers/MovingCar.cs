using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCar : MonoBehaviour
{
    [Header("Car Movement")]
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _startMovePosition = 30f;

    private GameManager _gameManager;
    private GameObject _player;

    private void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _player = GameObject.FindGameObjectWithTag("Player");
        GetComponent<AudioSource>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameManager.IsRunning && transform.position.z < _player.transform.position.z + _startMovePosition && transform.position.z > _player.transform.position.z -20)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - _moveSpeed * Time.deltaTime);
        }
    }
}
