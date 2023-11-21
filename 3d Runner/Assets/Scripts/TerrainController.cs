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
    [SerializeField] private GameObject _empty;

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

    public void OnRevive()
    {

        GameObject newEmpty = Instantiate(_empty, new Vector3(ActiveChunks[ActiveChunks.Count - 1].transform.position.x, ActiveChunks[ActiveChunks.Count - 1].transform.position.y, ActiveChunks[ActiveChunks.Count - 1].transform.position.z), Quaternion.identity, transform);
        ActiveChunks.Insert(ActiveChunks.Count, newEmpty);
        GameObject newEmpty2 = Instantiate(_empty, new Vector3(ActiveChunks[ActiveChunks.Count - 2].transform.position.x, ActiveChunks[ActiveChunks.Count - 2].transform.position.y, ActiveChunks[ActiveChunks.Count - 2].transform.position.z), Quaternion.identity, transform);
        ActiveChunks.Insert(ActiveChunks.Count, newEmpty2);
        for (int i = _maxLoadedChunks - 1; i >= 0; i--) {
            Destroy(ActiveChunks[i]);
            ActiveChunks.RemoveAt(i);
        }
        GameObject newGO = Instantiate(_empty, new Vector3(ActiveChunks[0].transform.position.x, ActiveChunks[0].transform.position.y, ActiveChunks[0].transform.position.z + _chunkLength), Quaternion.identity, transform);
        ActiveChunks.Insert(0, newGO);
    }
}
