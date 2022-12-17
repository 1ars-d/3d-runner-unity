using System.Collections;
using UnityEngine;

public class SwipeDetection : MonoBehaviour
{
    [SerializeField]
    private float minimumDistance = .2f;
    [SerializeField, Range(0f, 1f)]
    private float directionThreshold = .9f;

    private InputManager inputManager;

    private Vector2 startPosition;

    PlayerController playerController;

    private bool hasSwiped;

    private void Awake()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
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
        startPosition = position;
        hasSwiped = false;
    }

    private void SwipeEnd(Vector2 position, float time)
    {

    }

    private void Update()
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
        if (Vector2.Dot(Vector2.down, direction) > directionThreshold)
        {
            playerController.Roll();
        }
        if (Vector2.Dot(Vector2.left, direction) > directionThreshold)
        {
            playerController.GoLeft();
        }
        if (Vector2.Dot(Vector2.right, direction) > directionThreshold)
        {
            playerController.GoRight();
        }
    }
}
