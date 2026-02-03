using UnityEngine;
using Google.Protobuf;
using Protocol;
public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float sendInterval = 0.05f; // 0.05초 = 초당 20회 전송
    [SerializeField] private float moveThreshold = 0.01f; // 이 거리 이상 움직여야 전송 (최적화)
    [SerializeField] private float rotThreshold = 1.0f;   // 이 각도 이상 회전해야 전송

    private float _lastSendTime;
    private Vector3 _lastPos;
    private Quaternion _lastRot;

    private void Start()
    {
        _lastPos = transform.position;
        _lastRot = transform.rotation;
        _lastSendTime = Time.time;
    }

    private void Update()
    {
        // 1. 전송 주기 체크
        if (Time.time - _lastSendTime < sendInterval) 
            return;

        // 2. 변화량 체크 (가만히 있는데 보낼 필요 없음)
        bool isMoved = Vector3.Distance(transform.position, _lastPos) > moveThreshold;
        bool isRotated = Quaternion.Angle(transform.rotation, _lastRot) > rotThreshold;

        if (!isMoved && !isRotated) 
            return;

        // 3. 패킷 전송
        SendMovePacket();

        // 4. 마지막 상태 갱신
        _lastSendTime = Time.time;
        _lastPos = transform.position;
        _lastRot = transform.rotation;
    }

    private void SendMovePacket()
    {
        //C_Move packet = new C_Move();
        //packet.PosX = transform.position.x;
        //packet.PosY = transform.position.y;
        //packet.PosZ = transform.position.z;
        
        // FPS 게임은 어디를 보는지(Y축 회전)가 중요함
        //packet.RotY = transform.eulerAngles.y; 

        // NetworkManager를 통해 전송 (MsgId.C_MOVE는 가정)
        //NetworkManager.Instance.Send(packet, (int)MsgId.C_MOVE);
    }
}
