using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CruiserMotor : MonoBehaviour
{
    [Header("Driver")]
    public bool isDriver = true;
    public bool allowInput = true;

    [Header("Tuning")]
    public float accel = 14f;
    public float turnSpeedDeg = 75f;
    public float maxSpeed = 12f;
    public float brakeDrag = 4f;

    private Rigidbody rb;
    private CruiserMount mount;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mount = GetComponent<CruiserMount>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void FixedUpdate()
    {
        if (!isDriver || !allowInput) return;

        if (mount != null && !mount.IsSeatOccupied(0))
        {
            if (!rb.isKinematic) rb.isKinematic = true;
            return;
        }

        if (rb.isKinematic) rb.isKinematic = false;

        float throttle = Mathf.Clamp(Input.GetAxisRaw("Vertical"), -1f, 1f);
        float steer = Mathf.Clamp(Input.GetAxisRaw("Horizontal"), -1f, 1f);
        bool brake = Input.GetKey(KeyCode.Space);

        rb.AddForce(transform.forward * (throttle * accel), ForceMode.Acceleration);

        Vector3 v = rb.linearVelocity;
        Vector3 h = new Vector3(v.x, 0f, v.z);
        if (h.magnitude > maxSpeed)
        {
            Vector3 c = h.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(c.x, v.y, c.z);
        }

        float turn = steer * turnSpeedDeg * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));

        rb.linearDamping = brake ? brakeDrag : 0f;
    }
}
