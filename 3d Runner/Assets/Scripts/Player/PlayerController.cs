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
    [SerializeField] private float _kickPauseTime;
    [SerializeField] private float _kickJumpPower = 6f;
    [SerializeField] private float _yMult = .8f;
    [SerializeField] private float _jumpAfterTime = .5f;
    [SerializeField] private float _slopeJumpMultiplier = 1.5f;
    [HideInInspector] public bool IsOnSlope;
    private bool _jumpOnLand;

    private float _crouchAnimationTimer;
    private float _kickTimer;

    [Header("Positions")]
    [SerializeField] private float _leanPositionX;
    [SerializeField] private float _startPositionX;

    private float _yPos;
    private float _newXPos = 0f;
    public CharacterController m_char;
    private float _xPos;
    private float _colHeight;
    private float _colCenterY;
    private float _capsuleColHeight;
    private float _capsuleColCenterY;

    private Animator m_Animator;
    public bool _gameStarted;
    public bool InStartAnimation;

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
    public float _checkSlopeTimer;
    private bool _canJump;
    private bool _lastTouchingGround;
    private float _jumpTimer;
    private bool _fallingWater;
    public bool _inKick;

    [Header("Script and GameObject Refs")]
    [SerializeField] private CameraController _camController;
    [SerializeField] private GameObject _cop;
    [SerializeField] private GameObject _hitStars;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private GameObject _slopeCheck;
    [SerializeField] private LayerMask _slopesMask;
    [SerializeField] PlayerEffects _playerEffects;

    [Header("Chunk Speed")]
    [SerializeField] private float _terrainSpeed = 7f;
    [SerializeField] private float _accelrationDuration = 1f;
    [SerializeField] private bool _accelerate = true;
    [SerializeField] private float _accelerationIncrease;
    [SerializeField] private float _slopeIncrease = 2f;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip _switchSound;
    [SerializeField] private AudioClip _jumpSound;
    [SerializeField] private AudioClip _hitSound;
    [SerializeField] private AudioClip _landingSound;

    private float _currentSlopeIncrease;
    private float _currentSpeed;

    Animator _starsAnimator;
    CopController _copController;

    // Start is called before the first frame update
    void Start()
    {
        _copController = _cop.GetComponent<CopController>();
        _gameStarted = false;
        m_Animator = GetComponent<Animator>();
        _starsAnimator = _hitStars.GetComponent<Animator>();
        m_Animator.Play("Leaning");
        m_char = GetComponent<CharacterController>();
        _colHeight = m_char.height;
        _colCenterY = m_char.center.y;
        _capsuleColHeight = _playerCollider.height;
        _capsuleColCenterY = _playerCollider.center.y;
        m_Animator.SetFloat("RunSpeed", 1.1f);
        m_Animator.applyRootMotion = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_fallingWater)
        {
            Vector3 move = new Vector3(_xPos - transform.position.x, _yPos * _yMult * 3 * Time.deltaTime, 0);
            m_char.Move(move);
        }
        if (InStartAnimation)
        {
            Vector3 moveZVector = new Vector3(0, 0, (_currentSpeed + _currentSlopeIncrease) * Time.deltaTime);
            m_char.Move(moveZVector);
        } 
        if (!_gameStarted)
        {
            return;
        }
        if (_checkSlopeTimer <= 0)
        {
            bool _onSlopeNew = CheckOnSlope();
            if (!_onSlope && _onSlopeNew)
            {
                m_Animator.SetFloat("RunSpeed", 1.3f);
                StartCoroutine(TransitionSlopeIncrease(_slopeIncrease, 0.4f));
            }
            if (_onSlope && !_onSlopeNew)
            {
                m_Animator.SetFloat("RunSpeed", 1.1f);
                StartCoroutine(TransitionSlopeIncrease(0, 0.4f));
            }
            _onSlope = _onSlopeNew;
            _checkSlopeTimer = 0.15f;
        }
        if (Time.deltaTime <= 0)
        {
            return;
        }
        _checkSlopeTimer -= Time.deltaTime;
        float yMult = _yMult;
        if (_inJump)
            yMult = 1;
        Vector3 moveVector = new Vector3(_xPos - transform.position.x, _yPos * yMult * Time.deltaTime, (_currentSpeed + _currentSlopeIncrease) * Time.deltaTime);
        _xPos = Mathf.Lerp(_xPos, _newXPos, Time.deltaTime * _dodgeSpeed);
        m_char.Move(moveVector);
        HandleJump();
        HandleRoll();
        HandleStumble();
        HandleJumpTimer();
        HandleKick();
        SpeedIncrease();
    }

    private void SpeedIncrease()
    {
        if (_accelerate)
        {
            _currentSpeed += _accelerationIncrease * Time.fixedDeltaTime;
        }
    }

    private IEnumerator TransitionSlopeIncrease(float newIncrease, float duration)
    {
        float timeElapsed = 0;
        float startIncrease = _currentSlopeIncrease;
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);
            _currentSlopeIncrease = Mathf.Lerp(startIncrease, newIncrease, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        _currentSlopeIncrease = newIncrease;
    }

    private IEnumerator PlayKickSound(float delay)
    {
        SoundManager.Instance.PlaySound(_jumpSound);
        yield return null;
        //yield return new WaitForSeconds(delay);
        //SoundManager.Instance.PlaySound(_switchSound);
    }
    public void StartRunning()
    {
        StartCoroutine(GoToStartPosition(0.9f));
        m_Animator.CrossFadeInFixedTime("Running", .7f);
    }

    public void StartLookAnimation()
    {
        m_Animator.CrossFadeInFixedTime("LeaningLook", .1f);
    }

    IEnumerator GoToStartPosition(float duration)
    {
        InStartAnimation = true;
        float timeElapsed = 0;
        _slopeIncrease = 0f;
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);
            _currentSpeed = Mathf.Lerp(0, _terrainSpeed, t);
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(new Vector3(0, 90, 0)), Quaternion.Euler(new Vector3(0, 0, 0)), t);
            m_char.Move(new Vector3(Mathf.Lerp(_leanPositionX, _startPositionX, t) - transform.position.x, _yPos * _yMult * Time.deltaTime, 0));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = new Vector3(0, transform.position.y, transform.position.z);
        StartCoroutine(TransitionSlopeIncrease(1f, 2f));
        _gameStarted = true;
        InStartAnimation = false;
    }

    public void GoLeft()
    {
        if (!_gameStarted) return;
        _jumpOnLand = false;
        SoundManager.Instance.PlaySound(_switchSound);
        if (m_side == SIDE.Mid)
        {
            _newXPos = -_xWidth;
            m_last_side = SIDE.Mid;
            m_side = SIDE.Left;
            if (m_char.isGrounded && _crouchAnimationTimer <= 0)
            {
                m_Animator.CrossFadeInFixedTime("DodgeLeft", 0.15f);
                StartCoroutine(_copController.PlayAnimation("DodgeLeft", 0.15f));

            }
        }
        else if (m_side == SIDE.Right)
        {
            _newXPos = 0;
            m_last_side = SIDE.Right;
            m_side = SIDE.Mid;
            if (m_char.isGrounded && _crouchAnimationTimer <= 0)
            {
                m_Animator.CrossFadeInFixedTime("DodgeLeft", 0.15f);
                StartCoroutine(_copController.PlayAnimation("DodgeLeft", 0.15f));
            }
        }
        else
        {
            _newXPos = -2 * _xWidth;
            m_last_side = m_side;
            if (m_char.isGrounded && _crouchAnimationTimer <= 0)
            {
                m_Animator.CrossFadeInFixedTime("DodgeLeft", 0.15f);
                StartCoroutine(_copController.PlayAnimation("DodgeLeft", 0.15f));
            }
        }
    }

    public bool CheckOnSlope()
    {
        if (_inJump || !m_char.isGrounded)
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
        _jumpOnLand = false;
        SoundManager.Instance.PlaySound(_switchSound);
        if (m_side == SIDE.Mid)
        {
            _newXPos = _xWidth;
            m_last_side = SIDE.Mid;
            m_side = SIDE.Right;
            if (m_char.isGrounded && _crouchAnimationTimer <= 0)
            {
                m_Animator.CrossFadeInFixedTime("DodgeRight", 0.15f);
                StartCoroutine(_copController.PlayAnimation("DodgeRight", 0.15f));
            }
        }
        else if (m_side == SIDE.Left)
        {
            _newXPos = 0;
            m_last_side = SIDE.Left;
            m_side = SIDE.Mid;
            if (m_char.isGrounded && _crouchAnimationTimer <= 0)
            {
                 m_Animator.CrossFadeInFixedTime("DodgeRight", 0.15f);
                StartCoroutine(_copController.PlayAnimation("DodgeRight", 0.15f));
            }
        } else
        {
            _newXPos = 2 * _xWidth;
            m_last_side = m_side;
            if (m_char.isGrounded && _crouchAnimationTimer <= 0)
            {
                m_Animator.CrossFadeInFixedTime("DodgeRight", 0.15f);
                StartCoroutine(_copController.PlayAnimation("DodgeRight", 0.15f));
            }
        }
    }

    public void Kick()
    {
        if (!_gameStarted) return;
        if (_kickTimer <= 0)
        {
            StartCoroutine(PlayKickSound(.2f));
            m_Animator.CrossFadeInFixedTime("Kick", 0.1f);
            _inKick = true;
            _kickTimer = _kickPauseTime;
            _playerEffects.PlayerGlowUp(new Color(1, 0.7375139f, 0.25f, 1), 0.5f);
            if (!_inJump)
            {
                _inJump = true;
                if (CheckOnSlope())
                {
                    _yPos = _kickJumpPower * _slopeJumpMultiplier;
                }
                else
                {
                    _yPos = _kickJumpPower;
                }
            }
            
        }
    }

    private void HandleKick()
    {
        if (_kickTimer > 0 && _kickTimer - Time.deltaTime <= 0)
        {
            _inKick = false;
        } 
        _kickTimer -= Time.deltaTime;
    }

    private void HandleJumpTimer()
    {
        if (_lastTouchingGround && !m_char.isGrounded && !_inJump)
        {
            _jumpTimer = _jumpAfterTime;
        }
        _jumpTimer -= Time.deltaTime;
        _lastTouchingGround = m_char.isGrounded;
        _canJump = m_char.isGrounded || _jumpTimer > 0;
    }

    private void HandleJump()
    {
        if (m_char.isGrounded)
        {
            if (_isFalling)
            {
                if (_jumpOnLand && !_inRoll)
                {
                    _canJump = true;
                    Jump();
                    _jumpOnLand = false;
                    SoundManager.Instance.PlaySound(_landingSound);
                    return;
                }
                if (/*m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Falling") && */!_inRoll)
                {
                    if (CheckOnSlope())
                    {
                        m_Animator.CrossFadeInFixedTime("Running", 0.05f);
                        StartCoroutine(_copController.PlayAnimation("Running", 0.05f));
                    }
                    else
                    {
                        m_Animator.CrossFadeInFixedTime("Running", 0.15f);
                        StartCoroutine(_copController.PlayAnimation("Running", 0.05f));
                    }
                }
                _isFalling = false;
                _inJump = false;
                SoundManager.Instance.PlaySound(_landingSound);
            }
        }
        else
        {
            if (!_inJump && !_inRoll && m_char.velocity.y < -0.05f &&!m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Falling"))
            {
                m_Animator.CrossFadeInFixedTime("Falling", .2f);
                StartCoroutine(_copController.PlayAnimation("Falling", 0.2f));
            }
            _yPos -= _jumpPower * 2.7f * Time.fixedDeltaTime;
            if (m_char.velocity.y < -0.05f)
            {
                _isFalling = true;
            }
        }
    }

    public void Jump()
    {
        if (!_gameStarted) return;
        if (_canJump)
        {   
            if (CheckOnSlope())
            {
                _yPos = _jumpPower * _slopeJumpMultiplier;
            } else
            {
                _yPos = _jumpPower;
            }
            if (Random.Range(0, 2) == 0)
            {
                m_Animator.CrossFadeInFixedTime("Jump", 0.1f);
                StartCoroutine(_copController.PlayAnimation("Jump", 0.1f));
            }
            else
            {
                m_Animator.CrossFadeInFixedTime("JumpMirror", 0.1f);
                StartCoroutine(_copController.PlayAnimation("Jump", 0.1f));
            }
            SoundManager.Instance.PlaySound(_jumpSound);
            _inJump = true;
            _isFalling = false;
        } else if (_isFalling)
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
        _crouchAnimationTimer -= Time.deltaTime;
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
            SoundManager.Instance.PlaySound(_jumpSound);
            _crouchAnimationTimer = .5f;
            RollCounter = .3f;
            _playerCollider.height = _capsuleColHeight / 3f;
            _playerCollider.center = new Vector3(0, _capsuleColCenterY / 3f + .1f, 0);
            m_char.center = new Vector3(0, _colCenterY / 3f, 0);
            m_char.height = _colHeight / 3f;
            if (Random.Range(0, 10) == 0)
            {
                m_Animator.CrossFadeInFixedTime("Slide", 0.15f);
                StartCoroutine(_copController.PlayAnimation("Roll", 0.15f));
            }
            else
            {
                m_Animator.CrossFadeInFixedTime("Roll", 0.15f);
                StartCoroutine(_copController.PlayAnimation("Roll", 0.15f));
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
            StartCoroutine(_copController.LerpToDefault(2f));
            _stumbling = false;
            _starsAnimator.CrossFadeInFixedTime("fade_away", .1f);
        }
    }

    private void StumbleHit()
    {
        if (_nextStumbleDelay > 0 || !_gameManager.IsRunning) return;
        StartCoroutine(_camController.Shake(0.1f, 0.07f));
        if (_stumbleTimer > 0)
        {
            PlayerDie();
            return;
        }
        _nextStumbleDelay = .1f;
        _stumbling = true;
        StartCoroutine(_copController.LerpToNear(1f));
        _hitStars.SetActive(true);
        _starsAnimator.Play("stars_rotate");
        _stumbleTimer = _stumbleTime;
    }

    IEnumerator PlayDieAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        m_Animator.applyRootMotion = true;
        m_Animator.Play("Fall Over");
    }

    IEnumerator PlayFallOverAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        m_Animator.applyRootMotion = true;
        m_Animator.Play("FallFlat");
    }

    public void OnEnergyDead()
    {
        StartCoroutine(PlayFallOverAnimation(.001f));
        _gameManager.OnPlayerDied();
        _gameStarted = false;
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

    public void OnInstantWaterDeatHit()
    {
        _fallingWater = true;
        _gameManager.OnPlayerDied();
        _gameStarted = false;
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
        Debug.Log(_hitX);
        Debug.Log(_hitY);
        Debug.Log(_hitZ);

        if (_hitY == HitY.Low && _hitX == HitX.Mid) return;
        SoundManager.Instance.PlaySound(_hitSound);
        if (_hitY == HitY.Down)
        {
            m_char.Move(new Vector3(0, .35f, 0));
            StartCoroutine(_camController.Shake(0.13f, 0.1f));
            StumbleHit();
        }
        else if (_hitZ == HitZ.Forward && _hitX == HitX.Mid || _hitZ == HitZ.Mid && _hitX == HitX.Mid && !CheckOnSlope() && col.gameObject.tag != "Slope" && _hitY == HitY.Down) // Death
        {
            if (_inRoll && CheckOnSlope()) return;
            StartCoroutine(_camController.Shake(0.1f, 0.05f));
            PlayerDie();
        }
        else if (_hitZ == HitZ.Mid)
        {
            if (_hitX == HitX.Right || _hitX == HitX.Left)
            {
                //StartCoroutine(_camController.Shake(0.1f, 0.05f));
                HitSide();
            }
        }
        else
        {
            if (_hitX == HitX.Right || _hitX == HitX.Left)
            {
                StartCoroutine(_camController.Shake(0.13f, 0.1f));
                StumbleHit();
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
        if (average > col_bounds.size.x - 0.45f)
        {
            hit = HitX.Left;
        } else if (average < 0.45f)
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
        if (average < 0.05f)
        {
            hit = HitY.Low;
        }
        else if (average < 0.15f)
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
        if (average < 0.4f)
        {
            hit = HitZ.Backward;
        }
        else if (average < 0.6)
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
