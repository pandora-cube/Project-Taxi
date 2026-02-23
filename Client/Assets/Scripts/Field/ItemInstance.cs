using System.Collections.Generic;
using UnityEngine;

namespace Field
{
    public class ItemInstance : MonoBehaviour, IInteractable
    {
        private ItemSpawner _itemSpawner;
        private Item _item;

        public void Init(ItemSpawner spawner, Item item)
        {
            _itemSpawner = spawner;
            _item = item;
        }

        /// <summary>
        /// Character의 인벤토리에 아이템 추가
        /// </summary>
        void PickUpItem(Character character)
        {
            // TODO : LocalPlayer.AddItem으로 플레이어 아이템 획득
            Debug.Log($"{character.name} Item Picked");

            character.inventory.AddItem(_item);
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

        public InteractOption GetOption(Character character)
        {
            return new InteractOption { ActionName = "줍기", InteractAction = PickUpItem };
        }
    }
}

