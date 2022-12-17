using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    private Animator _animator;
    [Range(0, 100)]
    [SerializeField] private float _maxOffset = 0.4f;
    [SerializeField] private GameObject _collectEffect;

    private GameManager _gameManager;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
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
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _gameManager.AddCoin();
            float yPos = other.gameObject.transform.position.y + .9f;
            if (other.gameObject.GetComponent<PlayerController>()._inRoll)
            {
                yPos = yPos / 3;
            }
            Instantiate(_collectEffect, new Vector3(other.gameObject.transform.position.x, yPos, other.gameObject.transform.position.z + .15f), Quaternion.Euler(new Vector3(-90 + Random.Range(-90, 90), 90, 90)), other.gameObject.transform);
            Destroy(gameObject);
        }
    }
}
