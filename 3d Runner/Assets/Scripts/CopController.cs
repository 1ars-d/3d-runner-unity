using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopController : MonoBehaviour
{
    [Header("Cop Positions")]
    [SerializeField] private float _defaultZDistance = 5f;
    [SerializeField] private int _frameDistance = 50;
    [SerializeField] private float _animationDelay = 0.5f;
    [SerializeField] private Vector3 _endPos;
    [SerializeField] private Vector3 _endRotation;
    [SerializeField] private float _moveTime;
    [SerializeField] private PlayerController _player;
    [SerializeField] private GameManager _gameManager;

    // Follow Logics
    private List<Vector3> _storedPlayerPositions;
    private int _currentFollowDistance;
    private float _currentZDistance;
    private bool _lerpingToNear;

    Animator animator;

    private void Start()
    {
        _storedPlayerPositions = new List<Vector3>();
        _currentFollowDistance = _frameDistance;
        _currentZDistance = 5f;
        animator = GetComponent<Animator>();
        animator.SetFloat("RunSpeed", 1);
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;
        _storedPlayerPositions.Add(_player.transform.position);
        if (_storedPlayerPositions.Count > _currentFollowDistance)
        {
            transform.position = new Vector3(_storedPlayerPositions[0].x, _storedPlayerPositions[0].y, _storedPlayerPositions[0].z - _currentZDistance);
            _storedPlayerPositions.RemoveAt(0);
        }
    }
        

    private void PlayIdle()
    {
        animator.CrossFadeInFixedTime("Idle", 0.3f);
    }

    public IEnumerator LerpToNear(float duration)
    {
        float timeElapsed = 0;
        _lerpingToNear = true;
        float _startDistance = _currentZDistance;
        while (timeElapsed < duration)
        {
            _currentZDistance = Mathf.Lerp(_startDistance, 0, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        _currentFollowDistance = _frameDistance;
        _lerpingToNear = false;
    }

    public IEnumerator LerpToDeath(float duration)
    {
        float timeElapsed = 0;
        _lerpingToNear = true;
        float _startDistance = _currentZDistance;
        while (timeElapsed < duration)
        {
            if (transform.position.z > _player.transform.position.z - 2 && animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Stand")
            {
                animator.CrossFadeInFixedTime("Stand", .5f);

            }
            _currentZDistance = Mathf.Lerp(_startDistance, 0, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        _currentFollowDistance = _frameDistance;
        _lerpingToNear = false;
    }

    public IEnumerator LerpToDefault(float duration)
    {
        float timeElapsed = 0;
        float _startDistance = _currentZDistance;
        while (timeElapsed < duration && !_lerpingToNear)
        {
            _currentZDistance = Mathf.Lerp(_startDistance, _defaultZDistance, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        if (!_lerpingToNear)
            _currentZDistance = _defaultZDistance;
    }


    public void StartMoving()
    {
        animator.Play("Running");
    }

    public IEnumerator PlayAnimation(string name, float t)
    {
        yield return new WaitForSeconds(_animationDelay);
        animator.CrossFadeInFixedTime(name, t); 
    }

}
