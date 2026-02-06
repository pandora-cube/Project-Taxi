using System.Collections.Generic;
using UnityEngine;

namespace Field
{
    public class Item : MonoBehaviour , IInteractable
    {
        
        /// <summary>
        /// 아이템 획득
        /// </summary>
        void PickUpItem()
        {
            // TODO : LocalPlayer.AddItem으로 플레이어 아이템 획득
            Debug.Log("Item Picked");
            
            // TODO : 서버에 오브젝트 삭제 동기화
            Destroy(gameObject);
        }

        void DestroyItem()
        {
            Debug.Log("Item Destroyed");
            Destroy(gameObject);
        }
        
        void InspectItem()
        {
            Debug.Log("Item Information");
        }

        public List<InteractOption> GetOptions()
        {
            return new List<InteractOption>()
            {
                new InteractOption { ActionName = "줍기", InteractAction = PickUpItem },
                new InteractOption { ActionName = "파괴하기", InteractAction = DestroyItem },
                new InteractOption { ActionName = "정보", InteractAction = InspectItem }
            };
        }
    }
}

