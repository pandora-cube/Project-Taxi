using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 4.0f;
    [SerializeField] private float sprintSpeed = 7.0f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpGravityMultiplier = 2.0f; // 점프 후 떨어질 때 더 빠르게
    
    
    [Header("Survival Settings")]
    [SerializeField] private float sprintHungerThreshold = 20f; //달리기가 불가능해지는 허기 기준치
    
    [Header("Look Settings")]
    [SerializeField] private Transform playerCamera; // 자식으로 있는 메인 카메라
    [SerializeField] private float mouseSensitivity = 15.0f; // 마우스 감도
    [SerializeField] private float lookXLimit = 85.0f; // 목 꺾임 방지 (위아래 제한)

    // 내부 변수
    private CharacterController _controller;
    private PlayerControls _inputActions;
    private PlayerStats _playerStats;
    
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private Vector3 _velocity; // 중력/점프 처리를 위한 수직 속도
    private float _xRotation = 0f; // 카메라 상하 회전값 누적
    private bool _isSprinting;
    private bool _canSprint = true;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _inputActions = new PlayerControls();
        _playerStats = GetComponent<PlayerStats>();
        
        
        // 입력 이벤트 연결 (람다식 활용)
        _inputActions.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _inputActions.Player.Move.canceled += ctx => _moveInput = Vector2.zero;

        _inputActions.Player.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();
        _inputActions.Player.Look.canceled += ctx => _lookInput = Vector2.zero;

        _inputActions.Player.Sprint.performed += ctx => _isSprinting = true;
        _inputActions.Player.Sprint.canceled += ctx => _isSprinting = false;

        _inputActions.Player.Jump.performed += ctx => Jump();

        // 마우스 커서 숨기기 및 고정
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        _inputActions.Enable();
        _playerStats.OnHungerChanged += CheckSprintAvailability;
    }

    private void OnDisable()
    {
        _inputActions.Disable();
        _playerStats.OnHungerChanged -= CheckSprintAvailability;
    }
    

    private void Update()
    {
        HandleLook();
        HandleMovement();
        ApplyGravity();
    }
    
    //허기가 바뀔 때마다 호출되어 달리기 가능 여부를 갱신하는 함수
    private void CheckSprintAvailability(float currentHunger, float maxHunger)
    {
        // 현재 허기가 기준치 이상일 때만 달리기 가능
        _canSprint = currentHunger >= sprintHungerThreshold;

        // 만약 뛰고 있었는데 허기가 떨어져서 못 뛰게 되면 즉시 걷기로 전환
        if (!_canSprint && _isSprinting)
        {
            _isSprinting = false;
        }
    }

    // 1. 시점 처리 (마우스)
    private void HandleLook()
    {
        // 마우스 입력값 (설정된 감도와 Time.deltaTime 적용)
        // 주의: Input System의 Delta는 이미 프레임 보정이 되어있기도 하지만, 부드러움을 위해 deltaTime 곱하기도 함. 
        // Unity 6 Input System 기본 세팅이면 deltaTime을 곱하지 않는게 나을 수 있으니 테스트 필요.
        // 여기서는 감도 조절용으로만 곱함.
        float mouseX = _lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = _lookInput.y * mouseSensitivity * Time.deltaTime;

        // 상하 회전 (카메라만 돌림)
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -lookXLimit, lookXLimit);
        playerCamera.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        // 좌우 회전 (몸통 전체를 돌림)
        transform.Rotate(Vector3.up * mouseX);
    }

    // 2. 이동 처리 (키보드)
    private void HandleMovement()
    {
        // 현재 속도 결정 (Shift 누르면 달리기)
        float currentSpeed = _isSprinting ? sprintSpeed : walkSpeed;

        // 로컬 기준 이동 방향을 월드 기준으로 변환
        // transform.right = 플레이어의 오른쪽, transform.forward = 플레이어의 앞쪽
        Vector3 move = transform.right * _moveInput.x + transform.forward * _moveInput.y;

        // CharacterController로 이동 (중력 제외한 수평 이동)
        // Move는 '매 프레임 이동할 거리'를 받으므로 deltaTime 필수
        _controller.Move(move * currentSpeed * Time.deltaTime);
    }

    // 3. 점프 및 중력 처리
    private void ApplyGravity()
    {
        // 땅에 닿아있으면 수직 속도 초기화 (계속 떨어지는 것 방지)
        // isGrounded는 CharacterController가 제공하는 기능
        if (_controller.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f; // 0이 아니라 -2 정도로 눌러줘야 땅에 착 붙어있음 (계단/경사면 버그 방지)
        }

        // 중력 적용 (가속도 법칙: 속도 += 중력 * 시간)
        _velocity.y += gravity * Time.deltaTime;

        // 떨어질 때(점프 정점 이후) 더 빠르게 떨어지게 하여 타격감 주기
        if (_velocity.y < 0 && !_controller.isGrounded)
        {
            _velocity.y += gravity * (jumpGravityMultiplier - 1) * Time.deltaTime;
        }

        // 최종 수직 이동 적용
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void Jump()
    {
        // 땅에 있을 때만 점프 가능
        if (_controller.isGrounded)
        {
            // 점프 공식: v = sqrt(h * -2 * g)
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}