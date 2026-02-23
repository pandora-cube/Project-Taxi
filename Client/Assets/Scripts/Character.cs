using System.Collections.Generic;
using Field;
using UnityEngine;

public class Character : MonoBehaviour
{
    // TODO : 캐릭터 M 구현
    public Inventory inventory;

    public int hp;
    
    /// <summary>
    /// 현재 인벤토리의 들고있는 아이템을 사용
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool UseHeldItem(GameObject target)
    {
        var item = inventory.GetCurrentItem();
        Debug.Log(item);
        if (item == null) return false;
        if (item.TryUse(this, target))
        {
            return true;
        }

        return false;
    }
}