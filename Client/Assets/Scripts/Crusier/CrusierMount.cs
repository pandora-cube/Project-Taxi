using UnityEngine;

public class CruiserMount : MonoBehaviour
{
    [Header("Seat")]
    public Transform[] seats = new Transform[4];

    [Header("Players")]
    public Transform[] players = new Transform[4];
    public int localPlayerIndex = 0;

    [Header("Input")]
    public bool allowInput = true;
    public KeyCode interactKey = KeyCode.E;
    public float interactRadius = 1.5f;

    [Header("Seat Auto-Assign")]
    public bool autoAssignSeats = true;

    [Header("Dismount")]
    public float dismountSideOffset = 1.2f;
    public float dismountForwardOffset = 0.5f;

    private Transform[] occupants = new Transform[4];
    private Vector3[] originalLossyScale = new Vector3[4];
    private Collider cruiserCollider;

    private void Awake()
    {
        if (autoAssignSeats)
        {
            AutoAssignSeats();
        }

        cruiserCollider = GetComponent<Collider>();
        if (cruiserCollider == null)
        {
            cruiserCollider = GetComponentInChildren<Collider>();
        }

        EnsureOccupantArray();
    }

    private void Update()
    {
        if (!allowInput) return;

        Transform player = GetLocalPlayer();
        if (player == null) return;

        if (Input.GetKeyDown(interactKey))
        {
            HandleInteract(player);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) TrySwitchSeat(player, 0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TrySwitchSeat(player, 1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TrySwitchSeat(player, 2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) TrySwitchSeat(player, 3);
    }

    public bool Mount(Transform player, int seatIndex)
    {
        if (player == null) return false;
        if (!IsValidSeat(seatIndex)) return false;
        Transform seat = seats[seatIndex];
        if (seat == null) return false;

        EnsureOccupantArray();
        if (occupants[seatIndex] != null && occupants[seatIndex] != player) return false;

        var rb = player.GetComponent<Rigidbody>();
        var move = player.GetComponent<PlayerMove>();

        originalLossyScale[seatIndex] = player.lossyScale;

        player.SetParent(seat, worldPositionStays: true);
        player.position = seat.position;
        player.rotation = seat.rotation;

        Vector3 parentScale = seat.lossyScale;
        Vector3 targetWorld = originalLossyScale[seatIndex];
        player.localScale = new Vector3(
            parentScale.x != 0f ? targetWorld.x / parentScale.x : player.localScale.x,
            parentScale.y != 0f ? targetWorld.y / parentScale.y : player.localScale.y,
            parentScale.z != 0f ? targetWorld.z / parentScale.z : player.localScale.z);

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }
        if (move != null) move.enabled = false;

        occupants[seatIndex] = player;
        return true;
    }

    public bool Dismount(Transform player, Vector3 dropWorldPos)
    {
        if (player == null) return false;

        EnsureOccupantArray();
        int seatIndex = FindSeatIndex(player);
        if (seatIndex >= 0) occupants[seatIndex] = null;

        var rb = player.GetComponent<Rigidbody>();
        var move = player.GetComponent<PlayerMove>();

        player.SetParent(null, worldPositionStays: true);
        player.position = dropWorldPos;

       
        if (seatIndex >= 0)
        {
            player.localScale = originalLossyScale[seatIndex];
        }

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        if (move != null) move.enabled = true;

        return true;
    }

    public bool IsSeatOccupied(int seatIndex)
    {
        if (occupants == null || seatIndex < 0 || seatIndex >= occupants.Length) return false;
        return occupants[seatIndex] != null;
    }

    private void HandleInteract(Transform player)
    {
        int currentSeat = FindSeatIndex(player);
        if (currentSeat >= 0)
        {
            Vector3 drop = GetDropPosition(currentSeat);
            Dismount(player, drop);
            return;
        }

        // 근처에 있는 경우에만 탑승가능
        if (!IsAnySeatInRange(player, interactRadius) && !IsNearCruiser(player, interactRadius)) return;

        // 우선순위: Seat0, then Seat1, Seat2, Seat3
        int target = FindFirstAvailableSeatInOrder();
        if (target < 0) return;

        Mount(player, target);
    }

    private void TrySwitchSeat(Transform player, int targetSeatIndex)
    {
        if (!IsValidSeat(targetSeatIndex)) return;

        int currentSeat = FindSeatIndex(player);
        if (currentSeat < 0) return; // not inside

        if (occupants[targetSeatIndex] != null && occupants[targetSeatIndex] != player) return;
        if (currentSeat == targetSeatIndex) return;

        occupants[currentSeat] = null;
        Mount(player, targetSeatIndex);
    }

    private int FindFirstAvailableSeatInOrder()
    {
        for (int i = 0; i < seats.Length; i++)
        {
            if (seats[i] == null) continue;
            if (occupants[i] == null) return i;
        }
        return -1;
    }

    private bool IsAnySeatInRange(Transform player, float radius)
    {
        if (player == null || seats == null) return false;

        float r2 = radius * radius;
        Vector3 p = player.position;
        for (int i = 0; i < seats.Length; i++)
        {
            Transform seat = seats[i];
            if (seat == null) continue;
            if ((seat.position - p).sqrMagnitude <= r2) return true;
        }

        return false;
    }

    private bool IsNearCruiser(Transform player, float radius)
    {
        if (player == null) return false;

        if (cruiserCollider == null)
        {
            return (player.position - transform.position).sqrMagnitude <= radius * radius;
        }

        Vector3 closest = cruiserCollider.ClosestPoint(player.position);
        return (closest - player.position).sqrMagnitude <= radius * radius;
    }

    private Transform GetLocalPlayer()
    {
        if (players == null || players.Length == 0) return null;
        if (localPlayerIndex < 0 || localPlayerIndex >= players.Length) return null;
        return players[localPlayerIndex];
    }

    private void AutoAssignSeats()
    {
        if (seats == null || seats.Length != 4)
        {
            seats = new Transform[4];
        }

        for (int i = 0; i < seats.Length; i++)
        {
            if (seats[i] == null)
            {
                Transform seat = transform.Find($"Seat{i}");
                if (seat != null) seats[i] = seat;
            }
        }
    }

    private void EnsureOccupantArray()
    {
        if (occupants == null || occupants.Length != seats.Length)
        {
            occupants = new Transform[seats.Length];
        }
        if (originalLossyScale == null || originalLossyScale.Length != seats.Length)
        {
            originalLossyScale = new Vector3[seats.Length];
        }
    }

    private Vector3 GetDropPosition(int seatIndex)
    {
        if (!IsValidSeat(seatIndex) || seats[seatIndex] == null)
        {
            return transform.position + transform.right * 2f;
        }

        Transform seat = seats[seatIndex];
        float sideSign = Mathf.Sign(seat.localPosition.x);
        if (Mathf.Approximately(sideSign, 0f)) sideSign = 1f;

        return seat.position
            + seat.right * (dismountSideOffset * sideSign)
            + seat.forward * dismountForwardOffset;
    }

    private bool IsValidSeat(int seatIndex)
    {
        return seats != null && seatIndex >= 0 && seatIndex < seats.Length;
    }

    private int FindSeatIndex(Transform player)
    {
        if (occupants == null) return -1;
        for (int i = 0; i < occupants.Length; i++)
        {
            if (occupants[i] == player) return i;
        }
        return -1;
    }
}
