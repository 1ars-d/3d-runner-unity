using System.Collections;
using UnityEngine;

public class SwipeDetection : MonoBehaviour
{
    [Header("Double Tap")]
    [SerializeField] private float maxTapTime = .3f;
    [SerializeField] private float minTapTime = .1f;
    private float _currentTapTime;

    [Header("Swip")]
    [SerializeField] private float minimumDistance = .2f;
    [SerializeField, Range(0f, 1f)] private float directionThreshold = .9f;
    [SerializeField] private PlayerController playerController;

    private InputManager inputManager;

    private Vector2 startPosition;


    private bool hasSwiped;

    private void Awake()
    {
        inputManager = InputManager.Instance;
    }

    private void OnEnable()
    {
        inputManager.OnStartTouch += SwipeStart;
        inputManager.OnEndTouch += SwipeEnd;
    }

    private void OnDisable()
    {
        inputManager.OnStartTouch -= SwipeStart;
        inputManager.OnEndTouch -= SwipeEnd;
    }

    private void SwipeStart(Vector2 position, float time)
    {
        if (Time.timeScale == 0) return;
        startPosition = position;
        hasSwiped = false;
    }

    private void Update()
    {
        _currentTapTime -= Time.deltaTime;
    }

    private void SwipeEnd(Vector2 position, float time)
    {
        if (!hasSwiped)
        {
            if (_currentTapTime > 0 && maxTapTime - _currentTapTime > minTapTime)
            {
                playerController.Kick();
                _currentTapTime = 0;
            } else
            {
                _currentTapTime = maxTapTime;
            }
        }
    }

    private void FixedUpdate()
    {
        DetectSwipe();
    }

    private void DetectSwipe()
    {
        Vector2 currentPosition = inputManager.PrimaryPosition();
        if (Vector3.Distance(startPosition, currentPosition) >= minimumDistance && !hasSwiped)
        {
            hasSwiped = true;
            Vector3 direction = currentPosition - startPosition;
            Vector2 direction2D = new Vector2(direction.x, direction.y).normalized;
            SwipeDirection(direction2D);
        }
    }

    private void SwipeDirection(Vector2 direction)
    {
        if (Vector2.Dot(Vector2.up, direction) > directionThreshold)
        {
            playerController.Jump();
        }
        else if (Vector2.Dot(Vector2.down, direction) > directionThreshold)
        {
            playerController.Roll();
        }
        else if (Vector2.Dot(Vector2.left, direction) > directionThreshold)
        {
            playerController.GoLeft();
        }
        else if (Vector2.Dot(Vector2.right, direction) > directionThreshold)
        {
            playerController.GoRight();
        }
    }
}
