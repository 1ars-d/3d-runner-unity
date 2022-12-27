using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyController : MonoBehaviour
{

    [SerializeField] private float _energyValue;
    [SerializeField] private GameObject _collectEffect;

    private GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            float yPos = other.gameObject.transform.position.y + .9f;
            if (other.gameObject.GetComponent<PlayerController>()._inRoll)
            {
                yPos = yPos / 3;
            }
            _gameManager.OnEnergyCollect(_energyValue);
            Instantiate(_collectEffect, new Vector3(other.gameObject.transform.position.x, yPos, other.gameObject.transform.position.z + .15f), Quaternion.Euler(new Vector3(-90 + Random.Range(-90, 90), 90, 90)), other.gameObject.transform);
            Destroy(gameObject);
        }
    }
}
