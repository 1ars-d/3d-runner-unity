using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum SIDE { Left, Mid, Right }
public enum HitX { Left, Mid, Right, None }
public enum HitY { Up, Mid, Down, Low, None }
public enum HitZ { Forward, Mid, Backward, None }


[DefaultExecutionOrder(0)]
public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private SIDE m_side = SIDE.Mid;
    [SerializeField] private SIDE m_last_side = SIDE.Mid;
    [SerializeField] private float _dodgeSpeed;
    [SerializeField] private float _xWidth;
    [SerializeField] private float _jumpPower = 6f;
    [SerializeField] private bool _inJump;
    [SerializeField] public bool _inRoll;
    [SerializeField] private bool _isFalling;
    [SerializeField] private float _yMult = .8f;
    [HideInInspector] public bool IsOnSlope;
    private bool _jumpOnLand;

    [Header("Positions")]
    [SerializeField] private float _leanPositionX;
    [SerializeField] private float _startPositionX;

    private float _yPos;
    private float _newXPos = 0f;
    private CharacterController m_char;
    private float _xPos;
    private float _colHeight;
    private float _colCenterY;
    private float _capsuleColHeight;
    private float _capsuleColCenterY;

    private Animator m_Animator;
    public bool _gameStarted;

    [Header("Collision Detection")]
    [SerializeField] private HitX _hitX = HitX.None;
    [SerializeField] private HitY _hitY = HitY.None;
    [SerializeField] private HitZ _hitZ = HitZ.None;
    [SerializeField] private float _stumbleTime;
    [SerializeField] public CapsuleCollider _playerCollider;

    private float _stumbleTimer;
    private bool _stumbling;
    private float _nextStumbleDelay;

    public bool _onSlope;

    [Header("Script and GameObject Refs")]
    [SerializeField] private CameraController _camController;
    [SerializeField] private GameObject _cop;
    [SerializeField] private GameObject _hitStars;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private GameObject _slopeCheck;
    [SerializeField] private LayerMask _slopesMask;

    Animator _starsAnimator;
    CopController _copController;

    // Start is called before the first frame update
    void Start()
    {
        _copController = _cop.GetComponent<CopController>();
        _starsAnimator = _hitStars.GetComponent<Animator>();
        _gameStarted = false;
        m_Animator = GetComponent<Animator>();
        m_Animator.Play("Leaning");
        m_char = GetComponent<CharacterController>();
        _colHeight = m_char.height;
        _colCenterY = m_char.center.y;
        _capsuleColHeight = _playerCollider.height;
        _capsuleColCenterY = _playerCollider.center.y;
        m_Animator.SetFloat("RunSpeed", 1.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_gameStarted)
        {
            return;
        }
        bool _onSlopeNew = CheckOnSlope();
        if (!_onSlope && _onSlopeNew)
        {
            m_Animator.SetFloat("RunSpeed", 1.3f);
        }
        if (_onSlope && !_onSlopeNew)
        {
            m_Animator.SetFloat("RunSpeed", 1.1f);
        }
        _onSlope = _onSlopeNew;
        float yMult = _yMult;
        if (_inJump)
            yMult = 1;
        Vector3 moveVector = new Vector3(_xPos - transform.position.x, _yPos * yMult * Time.deltaTime, 0);
        _xPos = Mathf.Lerp(_xPos, _newXPos, Time.deltaTime * _dodgeSpeed);
        m_char.Move(moveVector);
        HandleJump();
        HandleRoll();
        HandleStumble();
    }

    public void StartRunning()
    {
        StartCoroutine(GoToStartPosition(1f));
        m_Animator.CrossFadeInFixedTime("Running", .7f);
    }

    IEnumerator GoToStartPosition(float duration)
    {
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(new Vector3(0, 90, 0)), Quaternion.Euler(new Vector3(0, 0, 0)), t);
            m_char.Move(new Vector3(Mathf.Lerp(_leanPositionX, _startPositionX, t) - transform.position.x, 0, 0));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void GoLeft()
    {
        if (!_gameStarted) return;
        if (m_side == SIDE.Mid)
        {
            _newXPos = -_xWidth;
            m_last_side = SIDE.Mid;
            m_side = SIDE.Left;
            if (m_char.isGrounded && !m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Roll") && !m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
            {
                m_Animator.CrossFadeInFixedTime("DodgeLeft", 0.15f);
            }
        }
        else if (m_side == SIDE.Right)
        {
            _newXPos = 0;
            m_last_side = SIDE.Right;
            m_side = SIDE.Mid;
            if (m_char.isGrounded && !m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Roll") && !m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
            {
                m_Animator.CrossFadeInFixedTime("DodgeLeft", 0.15f);
            }
        }
        else
        {
            _newXPos = -2 * _xWidth;
            m_last_side = m_side;
            if (m_char.isGrounded && !m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Roll") && !m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
            {
                m_Animator.CrossFadeInFixedTime("DodgeLeft", 0.15f);
            }
        }
    }

    public bool CheckOnSlope()
    {
        if (_inJump)
            return false;
        if (Physics.CheckSphere(_slopeCheck.transform.position, .3f, _slopesMask))
        {
            return true;
        }
        return false;
    }

    public void GoRight()
    {
        if (!_gameStarted) return;
        if (m_side == SIDE.Mid)
        {
            _newXPos = _xWidth;
            m_last_side = SIDE.Mid;
            m_side = SIDE.Right;
            if (m_char.isGrounded && !m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Roll") && !m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
            {
                m_Animator.CrossFadeInFixedTime("DodgeRight", 0.15f);
            }
        }
        else if (m_side == SIDE.Left)
        {
            _newXPos = 0;
            m_last_side = SIDE.Left;
            m_side = SIDE.Mid;
            if (m_char.isGrounded && !m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Roll") && !m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
            {
                 m_Animator.CrossFadeInFixedTime("DodgeRight", 0.15f);
            }
        } else
        {
            _newXPos = 2 * _xWidth;
            m_last_side = m_side;
            if (m_char.isGrounded && !m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Roll") && !m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
            {
                m_Animator.CrossFadeInFixedTime("DodgeRight", 0.15f);
            }
        }
    }

    private void HandleJump()
    {
        if (m_char.isGrounded)
        {
            if (_isFalling)
            {
                if (_jumpOnLand && !_inRoll)
                {
                    Jump();
                    _jumpOnLand = false;
                    return;
                }
                if (/*m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Falling") && */!_inRoll)
                {
                    m_Animator.CrossFadeInFixedTime("Running", 0.15f);
                }
                _isFalling = false;
                _inJump = false;
            }
        }
        else
        {
            if (!_inJump && !_inRoll && m_char.velocity.y < -0.05f &&!m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Falling"))
            {
                m_Animator.CrossFadeInFixedTime("Falling", .2f);
            }
            _yPos -= _jumpPower * 11.7f * Time.fixedDeltaTime;
            if (m_char.velocity.y < -0.05f)
            {
                _isFalling = true;
            }
        }
    }

    public void Jump()
    {
        if (!_gameStarted) return;
        if (m_char.isGrounded)
        {
            _yPos = _jumpPower;
            if (Random.Range(0, 2) == 0)
            {
                m_Animator.CrossFadeInFixedTime("Jump", 0.1f);
            } else
            {
                m_Animator.CrossFadeInFixedTime("JumpMirror", 0.1f);
            }

            _inJump = true;
            _isFalling = false;
        } else
        {
            _jumpOnLand = true;
        }
    }

    internal float RollCounter;

    private IEnumerator ResetColliders(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!_inRoll)
        {
            m_char.center = new Vector3(0, _colCenterY, 0);
            m_char.height = _colHeight;
            _playerCollider.height = _capsuleColHeight;
            _playerCollider.center = new Vector3(0, _capsuleColCenterY, 0);
        }
    }

    private void HandleRoll()
    {
        RollCounter -= Time.deltaTime;
        if (RollCounter <= 0f && _inRoll)
        {
            RollCounter = 0f;
            StartCoroutine(ResetColliders(.6f));
            _inRoll = false;
        }
    }

    public void Roll()
    {
        if (!_gameStarted) return;
        if (!m_char.isGrounded)
        {
            _yPos -= _jumpPower;
        }
        if (!_inRoll)
        {
            RollCounter = .3f;
            _playerCollider.height = _capsuleColHeight / 3f;
            _playerCollider.center = new Vector3(0, _capsuleColCenterY / 3f + .1f, 0);
            m_char.center = new Vector3(0, _colCenterY / 3f, 0);
            m_char.height = _colHeight / 3f;
            if (Random.Range(0, 10) == 0)
            {
                m_Animator.CrossFadeInFixedTime("Slide", 0.15f);
            } else
            {
                m_Animator.CrossFadeInFixedTime("Roll", 0.15f);
            }
            _inRoll = true;
        }
    }

    private void ResetCollisions()
    {
        _hitX = HitX.None;
        _hitY = HitY.None;
        _hitZ = HitZ.None;
    }

    private void HandleStumble()
    {
        if (_nextStumbleDelay > 0)
        {
            _nextStumbleDelay -= Time.deltaTime;
        }
        if (_stumbleTimer > 0)
        {
            _stumbleTimer -= Time.deltaTime;
        }
        if (_stumbling && _stumbleTimer <= 0)
        {
            _cop.SetActive(false);
            _stumbling = false;
            _starsAnimator.CrossFadeInFixedTime("fade_away", .1f);
        }
    }

    private void StumbleHit()
    {
        if (_nextStumbleDelay > 0) return;
        StartCoroutine(_camController.Shake(0.1f, 0.07f));
        if (_stumbleTimer > 0)
        {
            PlayerDie();
            return;
        }
        _nextStumbleDelay = .1f;
        _stumbling = true;
        _cop.SetActive(true);
        StartCoroutine(_copController.MoveTowardsPlayerStumble(1.5f));
        _hitStars.SetActive(true);
        _starsAnimator.Play("stars_rotate");
        _stumbleTimer = _stumbleTime;
    }

    IEnumerator PlayDieAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        m_Animator.Play("Fall Over");
    }

    private void PlayerDie()
    {
        StartCoroutine(PlayDieAnimation(.001f));
        _gameManager.OnPlayerDied();
        _gameStarted = false;
    }

    public void OnInstantDeatHit()
    {
        StartCoroutine(_camController.Shake(0.1f, 0.07f));
        PlayerDie();
    }

    public void OnMoveableObstacleHit()
    {
        StumbleHit();
    }

    private void HitSide()
    {
        StumbleHit();
        if (m_last_side == SIDE.Right && m_side == SIDE.Mid)
        {
            _newXPos = _xWidth;
            m_side = SIDE.Right;
            if (m_char.isGrounded && !_inRoll)
            {
                m_Animator.CrossFadeInFixedTime("DodgeRight", 0.15f);
            }
        }
        else if (m_last_side == SIDE.Left && m_side == SIDE.Mid)
        {
            _newXPos = -_xWidth;
            m_side = SIDE.Left;
            if (m_char.isGrounded && !_inRoll)
            {
                m_Animator.CrossFadeInFixedTime("DodgeLeft", 0.15f);
            }
        }
        else if (m_side == SIDE.Left && m_last_side == SIDE.Mid)
        {
            _newXPos = 0;
            m_side = SIDE.Mid;
            if (m_char.isGrounded && !_inRoll)
            {
                m_Animator.CrossFadeInFixedTime("DodgeRight", 0.15f);
            }
        }
        else if (m_side == SIDE.Right && m_last_side == SIDE.Mid)
        {
            _newXPos = 0;
            m_side = SIDE.Mid;
            if (m_char.isGrounded && !_inRoll)
            {
                m_Animator.CrossFadeInFixedTime("DodgeLeft", 0.15f);
            }
        }
        else if (m_last_side == SIDE.Right)
        {
            _newXPos = _xWidth;
            m_side = SIDE.Right;
            if (m_char.isGrounded && !_inRoll)
            {
                m_Animator.CrossFadeInFixedTime("DodgeLeft", 0.15f);
            }
        } else if (m_last_side == SIDE.Left)
        {
            _newXPos = -_xWidth;
            m_side = SIDE.Left;
            if (m_char.isGrounded && !_inRoll)
            {
                m_Animator.CrossFadeInFixedTime("DodgeRight", 0.15f);
            }
        }
    }

    public void OnCharacterCollideHit(Collider col)
    {
        if (!_gameStarted) return;
        _hitX = GetHitX(col);
        _hitY = GetHitY(col);
        _hitZ = GetHitZ(col);

        if (_hitY == HitY.Low && _hitX == HitX.Mid) return;
        if (_hitZ == HitZ.Forward && _hitX == HitX.Mid) // Death
        {
            StartCoroutine(_camController.Shake(0.1f, 0.05f));
            PlayerDie();
        }
        else if (_hitZ == HitZ.Mid)
        {
            if (_hitX == HitX.Right || _hitX == HitX.Left)
            {
                HitSide();
            }
        }
        else
        {
            if (_hitX == HitX.Right || _hitX == HitX.Left)
            {
                StartCoroutine(_camController.Shake(0.1f, 0.05f));
                PlayerDie();
            }
        }
        ResetCollisions();
    }

    public HitX GetHitX(Collider col)
    {
        Bounds char_bounds = m_char.bounds;
        Bounds col_bounds = col.bounds;
        float min_x = Mathf.Max(col_bounds.min.x, char_bounds.min.x);
        float max_x = Mathf.Min(col_bounds.max.x, char_bounds.max.x);
        float average = (min_x + max_x) / 2 - col_bounds.min.x;
        HitX hit;
        if (average > col_bounds.size.x - 0.33f)
        {
            hit = HitX.Left;
        } else if (average < 0.33)
        {
            hit = HitX.Right;
        } else
        {
            hit = HitX.Mid;
        }
        return hit;
    }

    public HitY GetHitY(Collider col)
    {
        Bounds char_bounds = m_char.bounds;
        Bounds col_bounds = col.bounds;
        float min_y = Mathf.Max(col_bounds.min.y, char_bounds.min.y);
        float max_y = Mathf.Min(col_bounds.max.y, char_bounds.max.y);
        float average = ((min_y + max_y) / 2 - char_bounds.min.y) / char_bounds.size.y;
        HitY hit;
        if (average < 0.15f)
        {
            hit = HitY.Low;
        }
        else if (average < 0.33f)
        {
            hit = HitY.Down;
        }
        else if (average < 0.66)
        {
            hit = HitY.Mid;
        }
        else
        {
            hit = HitY.Up;
        }
        return hit;
    }

    public HitZ GetHitZ(Collider col)
    {
        Bounds char_bounds = m_char.bounds;
        Bounds col_bounds = col.bounds;
        float min_z = Mathf.Max(col_bounds.min.z, char_bounds.min.z);
        float max_z = Mathf.Min(col_bounds.max.z, char_bounds.max.z);
        float average = ((min_z + max_z) / 2 - char_bounds.min.z) / char_bounds.size.z;
        HitZ hit;
        if (average < 0.2f)
        {
            hit = HitZ.Backward;
        }
        else if (average < 0.8)
        {
            hit = HitZ.Mid;
        }
        else
        {
            hit = HitZ.Forward;
        }
        return hit;
    }

}
