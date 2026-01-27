using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    
    [Header("Mouse Sensitivity")]
    public float mouseSensitivity = 200f;
    public Transform cameraTransform; // 플레이어의 자식인 카메라

    private Rigidbody rb;
    private Vector3 moveDirection;
    private float xRotation = 0f;

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
        // --- 1. 이동 입력 ---
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        moveDirection = (transform.forward * v + transform.right * h).normalized;

        // --- 2. 회전 입력 ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 좌우 회전 (몸통)
        transform.Rotate(Vector3.up * mouseX);

        // 상하 회전 (카메라)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }
}