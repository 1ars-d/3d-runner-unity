using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyItemSpawner : MonoBehaviour
{

    [SerializeField] private GameObject _replaceCoin;
    [SerializeField] private GameObject _EnergyItem;

    private GameManager _gameManager;
    

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (_gameManager.EnergyItemCount <= 0)
        {
            _replaceCoin.SetActive(false);
            _EnergyItem.SetActive(true);
            _gameManager.EnergyItemCount = Random.Range(_gameManager.EnergyItemRate - 1, _gameManager.EnergyItemRate + 1);
        } else
        {
            _gameManager.EnergyItemCount--;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
