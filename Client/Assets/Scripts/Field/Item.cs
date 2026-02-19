using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Field
{
    [CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
    public class Item : ScriptableObject
    {
        [SerializeField] private string itemName;
        public string ItemName => itemName;
        
        [SerializeField] private ItemEffect  itemEffect;

        /// <summary>
        /// 해당 아이템 사용
        /// </summary>
        /// <param name="user">사용자</param>
        /// <param name="target">현재 바라보는 대상</param>
        /// <returns>사용 여부 반환</returns>
        public bool TryUse(Character user, GameObject target)
        {
            if (!itemEffect) return false;
            
            if (target && itemEffect.CanUseOnTarget && itemEffect.CanUseOn(target))
            {
                itemEffect.Execute(user, target);
                user.inventory.RemoveItem(this);
                return true;
            }

            if (itemEffect.CanUseOnSelf)
            {
                itemEffect.Execute(user, null);
                user.inventory.RemoveItem(this);
                return true;
            }

            return false;
        }
        

    }


    public abstract class ItemEffect : ScriptableObject
    {
        // TODO : 사용 방법 별로 상속받은 클래스 만들기 및 필요 시 Item과 병합
        [SerializeField] private string actionName;
        public string ActionName => actionName;
        [SerializeField] private bool canUseOnTarget;
        public bool CanUseOnTarget => canUseOnTarget;
        [SerializeField] private bool canUseOnSelf;
        public bool CanUseOnSelf => canUseOnSelf;

        /// <summary>
        /// 이 효과가 target에게 사용가능한지 확인
        /// </summary>
        /// <param name="target">현재 상호작용하려는 오브젝트</param>
        /// <returns>효과 사용 가능 여부</returns>
        public abstract bool CanUseOn(GameObject target);
        /// <summary>
        /// user가 이 효과를 target에게 사용
        /// </summary>
        /// <param name="user">사용자</param>
        /// <param name="target">상호작용하는 오브젝트</param>
        public abstract void Execute(Character user,GameObject target);
    } 
}
