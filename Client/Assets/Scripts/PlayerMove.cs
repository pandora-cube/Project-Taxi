using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    
    [Header("Mouse Sensitivity")]
    public float mouseSensitivity = 200f;
    public Transform cameraTransform; // 플레이어의 자식인 카메라

    public float jumpForce = 7f;     // 점프 힘
    private Rigidbody rb;
    private Vector3 moveDirection;
    private float xRotation = 0f;

    [Header("Ground Check")]
    public bool isGrounded;          // 바닥에 닿아있는지 확인
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;    // 바닥으로 인식할 레이어

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // 핵심: 마우스 커서를 화면 중앙에 강제로 고정하고 숨깁니다.
        // 이렇게 해야 마우스를 움직였을 때 커서가 밖으로 나가지 않고,
        // 화면 중앙이 곧 마우스 포인터의 위치가 됩니다.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // 커서 숨김
    }

    void Update()
    {


        // 회전 입력 
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 좌우 회전 (몸통)
        transform.Rotate(Vector3.up * mouseX);

        // 상하 회전 (카메라)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        // WASD 입력 받기
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 이동 방향 계산 (캐릭터가 바라보는 방향 기준)
        moveDirection = (transform.forward * v + transform.right * h).normalized;

        // 바닥 체크 (레이캐스트)
        CheckGround();

        // 점프 입력 (바닥일 때만)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void CheckGround()
    {
        // 캐릭터 중심에서 아래로 레이를 쏴서 바닥 레이어와 충돌하는지 확인
        isGrounded = Physics.Raycast(transform.position, Vector3.down, (GetComponent<CapsuleCollider>().height * 0.5f) + groundCheckDistance, groundLayer);
        
        // 에디터 뷰에서 바닥 체크 레이를 시각적으로 표시 (선택사항)
        Debug.DrawRay(transform.position, Vector3.down * ((GetComponent<CapsuleCollider>().height * 0.5f) + groundCheckDistance), isGrounded ? Color.green : Color.red);
    }

    void Jump()
    {
        // Y축 속도를 초기화하고 점프 힘 가하기 (일관된 점프 높이 유지)
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        // 5. 물리 이동
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }
}