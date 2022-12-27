using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    #region Events
    public delegate void StartTouch(Vector2 position, float time);
    public event StartTouch OnStartTouch;
    public delegate void EndTouch(Vector2 position, float time);
    public event StartTouch OnEndTouch;
    #endregion

    private PlayerControlls playerControlls;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        playerControlls = new PlayerControlls();
    }

    private void OnEnable()
    {
        playerControlls.Enable();
    }

    private void OnDisable()
    {
        playerControlls.Disable();
    }


    // Start is called before the first frame update
    void Start()
    {
        playerControlls.Touch.PrimaryContact.started += ctx => StartTouchPrimary(ctx);
        playerControlls.Touch.PrimaryContact.canceled += ctx => EndTouchPrimary(ctx);
    }

    private void StartTouchPrimary(InputAction.CallbackContext context)
    {
        if (OnStartTouch != null)
        {
            OnStartTouch(playerControlls.Touch.PrimaryPosition.ReadValue<Vector2>(), (float)context.startTime);
        }
    }

    private void EndTouchPrimary(InputAction.CallbackContext context)
    {
        if (OnStartTouch != null)
        {
            OnEndTouch(playerControlls.Touch.PrimaryPosition.ReadValue<Vector2>(), (float)context.time);
        }
    }

    public Vector2 PrimaryPosition()
    {
        return playerControlls.Touch.PrimaryPosition.ReadValue<Vector2>();
    }


}
