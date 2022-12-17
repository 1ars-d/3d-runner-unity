using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainController : MonoBehaviour
{
    [Header("Chunks")]
    [SerializeField] private int _maxLoadedChunks = 5;
    [SerializeField] private float _chunkLength;
    [SerializeField] private List<GameObject> _chunksList;
    [SerializeField] public List<GameObject> ActiveChunks;

    [Header("Chunk Speed")]
    [SerializeField] private float _terrainSpeed = 7f;
    [SerializeField] private float _accelrationDuration = 1f;
    [SerializeField] private bool _accelerate = true;
    [SerializeField] private float _slopeIncrease = 2f;
    private float _currentSlopeIncrease;
    private float _currentSpeed;

    private float _elapsedTime;

    GameManager gameManager;
    PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.IsRunning) return;
        if (playerController.CheckOnSlope())
            _currentSlopeIncrease = _slopeIncrease;
        else
            _currentSlopeIncrease = 0;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - (_currentSpeed + _currentSlopeIncrease) * Time.deltaTime);
        SpawnTiles();
    }

    private void FixedUpdate()
    {
        if (_accelerate)
        {
            _currentSpeed += .0003f;
        }
    }

    public void StartMoving()
    {
        StartCoroutine(AccelerateSpeed(_accelrationDuration));
    }

    public void StopMoving()
    {
        _currentSpeed = 0;
    }

    IEnumerator AccelerateSpeed(float duration)
    {
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);
            _currentSpeed = Mathf.Lerp(0, _terrainSpeed, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void SpawnTiles()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= 0.5 && ActiveChunks.Count < _maxLoadedChunks)
        {
            GameObject newGO = Instantiate(_chunksList[Random.Range(0, _chunksList.Count)], new Vector3(ActiveChunks[0].transform.position.x, ActiveChunks[0].transform.position.y, ActiveChunks[0].transform.position.z + _chunkLength), Quaternion.identity, transform);
            ActiveChunks.Insert(0, newGO);
            _elapsedTime = 0;
        }
    }
}
