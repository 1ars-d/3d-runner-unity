using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningSoundsController : MonoBehaviour
{
    private PlayerController _playerController;
    private AudioSource _audioSource;

    [SerializeField] private float volume;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerController.m_char.isGrounded)
        {
            _audioSource.volume = volume;
        } else
        {
            _audioSource.volume = 0f;
        }
    }
}
