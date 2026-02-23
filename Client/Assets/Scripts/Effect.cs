using UnityEngine;

public abstract class Effect : ScriptableObject
{
    // 아이템을 사용할 수 있는지 체크하는 함수
    public abstract bool CanUseOn(GameObject target);

    // 실제 아이템 효과를 실행하는 함수
    // user: 아이템을 사용 주체 
    // target: 아이템 효과 받는 대상
    public abstract void Execute(PlayerStat user, GameObject target);
}