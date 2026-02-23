using UnityEngine;
using System;
public class PlayerStats : MonoBehaviour
{
[Header("Network Authority")]
    // 이 캐릭터가 LocalPlayer인지 확인
    public bool isLocalPlayer = true;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Hunger Settings")]
    [SerializeField] private float maxHunger = 100f;
    [SerializeField] private float hungerDecreaseRate = 1.0f; // 1초당 닳는 허기 수치
    [SerializeField] private float currentHunger;

    [Header("Starvation Penalty")]
    [SerializeField] private float starvationDamage = 2.0f; // 허기가 0일 때 1초당 입는 체력 피해

    // UI나 다른 시스템에 변경 사항을 알리는 이벤트
    public event Action<float, float> OnHealthChanged; //현재 체력, 최대 체력
    public event Action<float, float> OnHungerChanged; //현재 허기, 최대 허기
    public event Action OnPlayerDied;

    private bool _isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        currentHunger = maxHunger;
        
        // UI갱신을 위한 Invoke
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnHungerChanged?.Invoke(currentHunger, maxHunger);
    }

    private void Update()
    {
        if (_isDead) return;
        
        if (isLocalPlayer)
        {
            HandleHunger();
        }
    }

    private void HandleHunger()
    {
        if (currentHunger > 0)
        {
            currentHunger -= hungerDecreaseRate * Time.deltaTime;
            currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
            
            OnHungerChanged?.Invoke(currentHunger, maxHunger);
        }
        else
        {
            TakeDamage(starvationDamage * Time.deltaTime);
        }
    }
    

    public void TakeDamage(float damageAmount)
    {
        // 남의 캐릭터는 스스로 데미지를 계산하지 않는다.
        if (_isDead || !isLocalPlayer) return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        // TODO: 여기서 네트워크 담당자가 "나 데미지 입었어!" 하고 서버로 패킷을 쏩니다.

        if (currentHealth <= 0) Die();
    }

    public void Heal(float healAmount)
    {
        if (_isDead || !isLocalPlayer) return;

        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void Eat(float foodAmount)
    {
        if (_isDead || !isLocalPlayer) return;

        currentHunger += foodAmount;
        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
        
        OnHungerChanged?.Invoke(currentHunger, maxHunger);
    }

    private void Die()
    {
        _isDead = true;
        Debug.Log("플레이어 사망");
        OnPlayerDied?.Invoke();
    }

    //Remote를 동기화할 때 쓰이는 전용 함수
    //서버에서 동료의 체력/허기 패킷을 받으면 이 함수 호출
    public void SyncStatsFromServer(float serverHealth, float serverHunger)
    {
        if (isLocalPlayer) return; 
        
        currentHealth = serverHealth;
        currentHunger = serverHunger;

        // UI갱신을 위한 Invoke
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnHungerChanged?.Invoke(currentHunger, maxHunger);
    }
}
