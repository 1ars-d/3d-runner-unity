using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkController : MonoBehaviour
{
    private Transform _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (transform.position.z < _player.transform.position.z - 60f)
        {
            gameObject.GetComponentInParent<TerrainController>().ActiveChunks.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}
