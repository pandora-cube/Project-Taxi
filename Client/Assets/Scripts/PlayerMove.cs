using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 1. WASD 입력 받기 (Horizontal: A/D, Vertical: W/S)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 2. 이동 방향 계산
        moveDirection = new Vector3(h, 0, v).normalized;
    }

    void FixedUpdate()
    {
        // 3. 물리 엔진을 이용한 실제 이동
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }
}