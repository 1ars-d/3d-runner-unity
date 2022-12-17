using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCar : MonoBehaviour
{
    [Header("Car Movement")]
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _startMovePosition = 30f;

    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameManager.IsRunning && transform.position.z < _startMovePosition && transform.position.z > -20)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - _moveSpeed * Time.deltaTime);
        }
    }
}
