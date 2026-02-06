using UnityEngine;

public class RemotePlayerMover : MonoBehaviour
{
    [Header("Smoothing")]
    [SerializeField] private float lerpSpeed = 10.0f; // 보간 속도 (높을수록 빠릿함, 낮을수록 부드러움)

    // 목표 지점 (서버가 알려준 최신 위치)
    private Vector3 _targetPos;
    private Quaternion _targetRot;

    private void Start()
    {
        // 초기화: 시작하자마자 0,0,0에서 날아오지 않게 현재 위치로 설정
        _targetPos = transform.position;
        _targetRot = transform.rotation;
    }

    // ① [NetworkManager]가 패킷을 받으면 이 함수를 호출해줌
    public void SetTarget(float x, float y, float z/*, float rotY*/)
    {
        _targetPos = new Vector3(x, y, z);
        _targetRot = Quaternion.Euler(0, 0/*rotY*/, 0);
    }

    // ② [Update] 매 프레임 부드럽게 이동 (Interpolation)
    private void Update()
    {
        // 위치 보간 (Lerp)
        // Time.deltaTime * lerpSpeed를 쓰면 프레임이 튀어도 부드럽게 따라감
        transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * lerpSpeed);

        // 회전 보간 (Slerp) - 회전은 Slerp가 자연스러움
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, Time.deltaTime * lerpSpeed);
    }
}