using UnityEngine;

[CreateAssetMenu(fileName = "NewFood", menuName = "Items/Food")]
public class Food : Effect
{
    [Header("회복 수치 설정")]
    public int hpRegen = 20;
    public int hungerRegen = 20;

    public override bool CanUseOn(GameObject target) 
    { 
        // 대상에게 PlayerStat 컴포넌트 유무 판별
        return target.GetComponent<PlayerStat>() != null; 
    }

    public override void Execute(PlayerStat user, GameObject target)
    {
        // 최대치 제한한 회복
        user.hp = Mathf.Min(user.hp + hpRegen, user.maxHp);
        user.hunger = Mathf.Min(user.hunger + hungerRegen, user.maxHunger);
    }
}