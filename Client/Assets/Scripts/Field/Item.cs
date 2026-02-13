using System.Collections.Generic;
using UnityEngine;

namespace Field
{
    public class Item : MonoBehaviour, IInteractable
    {
        private ItemSpawner _itemSpawner;

        public void Init(ItemSpawner spawner)
        {
            _itemSpawner = spawner;
        }

        /// <summary>
        /// Character의 인벤토리에 아이템 추가
        /// </summary>
        void PickUpItem(Character character)
        {
            // TODO : LocalPlayer.AddItem으로 플레이어 아이템 획득
            Debug.Log($"{character.name} Item Picked");

            // TODO : 서버에 오브젝트 삭제 동기화
            _itemSpawner.ReleaseObject(this);
        }

        void DestroyItem(Character character)
        {
            Debug.Log($"{character.name} Item Destroyed");
            _itemSpawner.ReleaseObject(this);
        }

        void InspectItem(Character character)
        {
            Debug.Log($"{character.name} Item Information");
        }

        public List<InteractOption> GetOptions(Character character)
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

