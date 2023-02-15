using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainController : MonoBehaviour
{
    [Header("Chunks")]
    [SerializeField] private int _maxLoadedChunks = 5;
    [SerializeField] private float _chunkLength;
    [SerializeField] private List<GameObject> _chunkList;
    [SerializeField] public List<GameObject> ActiveChunks;

    [Header("Scripts")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerController playerController;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        SpawnTiles();
    }

    private void SpawnTiles()
    {
        if (ActiveChunks.Count < _maxLoadedChunks)
        {
            GameObject newGO = Instantiate(_chunkList[Random.Range(0, _chunkList.Count)], new Vector3(ActiveChunks[0].transform.position.x, ActiveChunks[0].transform.position.y, ActiveChunks[0].transform.position.z + _chunkLength), Quaternion.identity, transform);
            ActiveChunks.Insert(0, newGO);
        }
    }
}
