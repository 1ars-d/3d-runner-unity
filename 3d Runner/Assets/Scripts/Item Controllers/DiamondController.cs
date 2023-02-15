using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondController : MonoBehaviour
{

    [SerializeField] private GameObject _collectEffect;

    private GameObject _player;
    private PlayerController _playerController;

    private GameManager _gameManager;
    private PowerUpManager _PUManager;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _PUManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PowerUpManager>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerController = _player.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _gameManager.AddDiamond();
            _gameManager.OnDiamondCollect();
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
