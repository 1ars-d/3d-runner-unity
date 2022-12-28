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
    [SerializeField] private float _accelerationIncrease;
    [SerializeField] private float _slopeIncrease = 2f;
    private float _currentSlopeIncrease;
    private float _currentSpeed;
    private bool _lastOnSlope;

    private float _elapsedTime;

    [Header("Scripts")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerController playerController;

    // Start is called before the first frame update
    private void Start()
    {
        _lastOnSlope = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.IsRunning) return;
        SlopeCheck();
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - (_currentSpeed + _currentSlopeIncrease) * Time.deltaTime);
        SpawnTiles();
    }
    private void FixedUpdate()
    {
        if (_accelerate)
        {
            _currentSpeed += _accelerationIncrease;
        }
    }

    private void SlopeCheck()
    {
        bool currentOnSlope = playerController.CheckOnSlope();
        if (!_lastOnSlope && currentOnSlope)
            StartCoroutine(TransitionSlopeIncrease(_slopeIncrease, 0.4f));
        else if (_lastOnSlope && !currentOnSlope)
            StartCoroutine(TransitionSlopeIncrease(0, 0.4f));
        _lastOnSlope = currentOnSlope;
    }


    private IEnumerator TransitionSlopeIncrease(float newIncrease, float duration)
    {
        float timeElapsed = 0;
        float startIncrease = _currentSlopeIncrease;
        while (timeElapsed < duration && gameManager.IsRunning)
        {
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);
            _currentSlopeIncrease = Mathf.Lerp(startIncrease, newIncrease, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        _currentSlopeIncrease = newIncrease;
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
