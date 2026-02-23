using UnityEngine;
using System.Collections;

public class PlayerStat : MonoBehaviour
{
    [Header("Status Values")]
    public int hp = 100;
    public int maxHp = 100;
    public int hunger = 100;
    public int maxHunger = 100;
    public int thirst = 100;
    public int maxThirst = 100;

    void Start()
    {
        StartCoroutine(SurvivalRoutine());
    }

    IEnumerator SurvivalRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            hunger = Mathf.Max(hunger - 5, 0);
            thirst = Mathf.Max(thirst - 7, 0);

            // 허기나 갈증 중 하나라도 0이면 체력 감소 시작
            if (hunger <= 0 || thirst <= 0)
            {
                // 둘 다 0이면 페널티 강화
                int penalty = (hunger <= 0 && thirst <= 0) ? 10 : 5;
                hp -= penalty;
                hp = Mathf.Clamp(hp, 0, maxHp);
            }

            // 사망 체크
            if (hp <= 0)
            {
                // 사망 처리 로직 (이후 루프 중단 등)
                yield break;
            }
        }
    }

    // 회복 메서드도 Clamp를 사용하여 max를 넘지 않게 처리
    public void Restore(int hpAmount, int hungerAmount, int thirstAmount)
    {
        hp = Mathf.Clamp(hp + hpAmount, 0, maxHp);
        hunger = Mathf.Clamp(hunger + hungerAmount, 0, maxHunger);
        thirst = Mathf.Clamp(thirst + thirstAmount, 0, maxThirst);
    }
}