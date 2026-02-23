using UnityEngine;

[CreateAssetMenu(fileName = "NewDrink", menuName = "Items/Drink")]
public class Drink : Effect
{
    [Header("회복 수치 설정")]
    public int thirstRegen = 30;
    public int hpRegen = 0;

    public override bool CanUseOn(GameObject target) 
    { 
        return target.GetComponent<PlayerStat>() != null; 
    }

    public override void Execute(PlayerStat user, GameObject target)
    {
        PlayerStat targetStatus = target.GetComponent<PlayerStat>();
        if (targetStatus != null)
        {
            targetStatus.Restore(hpRegen, 0, thirstRegen);
        }
    }
}