using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum TERRAIN { CITY, HALFCITY, BRIDGE }

public class TerrainController : MonoBehaviour
{
    [Header("Chunks")]
    [SerializeField] private int _maxLoadedChunks = 5;
    [SerializeField] private float _chunkLength;
    [SerializeField] private List<GameObject> _cityChunksList;
    [SerializeField] private List<GameObject> _bridgeChunksList;
    [SerializeField] private List<GameObject> _halfCityChunksList;
    [SerializeField] private GameObject _cityToBridge;
    [SerializeField] private GameObject _bridgeToCity;
    [SerializeField] private GameObject _cityToHalf;
    [SerializeField] public List<GameObject> ActiveChunks;
    [SerializeField] private int _terrainCountRangeMin = 5;
    [SerializeField] private int _terrainCountRangeMax = 10;

    private TERRAIN _currentTerrain;
    [SerializeField] private int _changeTerrainCount;

    private float _elapsedTime;

    [Header("Scripts")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerController playerController;

    // Start is called before the first frame update
    private void Start()
    {
        _currentTerrain = TERRAIN.CITY;
        _changeTerrainCount = Random.Range(_terrainCountRangeMin, _terrainCountRangeMax);
    }

    // Update is called once per frame
    void Update()
    {
        SpawnTiles();
    }

    private void ChangeTerrain()
    {
        if (_currentTerrain == TERRAIN.CITY)
        {
            GameObject newGO = Instantiate(_cityToHalf, new Vector3(ActiveChunks[0].transform.position.x, ActiveChunks[0].transform.position.y, ActiveChunks[0].transform.position.z + _chunkLength), Quaternion.identity, transform);
            ActiveChunks.Insert(0, newGO);
            _currentTerrain = TERRAIN.HALFCITY;
        }
        else if (_currentTerrain == TERRAIN.BRIDGE)
        {
            GameObject newGO = Instantiate(_bridgeToCity, new Vector3(ActiveChunks[0].transform.position.x, ActiveChunks[0].transform.position.y, ActiveChunks[0].transform.position.z + _chunkLength), Quaternion.identity, transform);
            ActiveChunks.Insert(0, newGO);
            _currentTerrain = TERRAIN.CITY;
        }
        _changeTerrainCount = Random.Range(_terrainCountRangeMin, _terrainCountRangeMax);
    }

    private void SpawnTiles()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= 0.5 && ActiveChunks.Count < _maxLoadedChunks)
        {
            Random.seed = System.DateTime.Now.Millisecond;
            List<GameObject> _chunksList = new List<GameObject>();
            if (_currentTerrain == TERRAIN.CITY)
                _chunksList = _cityChunksList;
            if (_currentTerrain == TERRAIN.HALFCITY)
                _chunksList = _halfCityChunksList;
            if (_currentTerrain == TERRAIN.BRIDGE)
                _chunksList = _bridgeChunksList;
            GameObject newGO = Instantiate(_chunksList[Random.Range(0, _chunksList.Count)], new Vector3(ActiveChunks[0].transform.position.x, ActiveChunks[0].transform.position.y, ActiveChunks[0].transform.position.z + _chunkLength), Quaternion.identity, transform);
            ActiveChunks.Insert(0, newGO);
            _elapsedTime = 0;
            _changeTerrainCount -= 1;
            if (_changeTerrainCount <= 0)
            {
                ChangeTerrain();
            }
        }
    }
}
