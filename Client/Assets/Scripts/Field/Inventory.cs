using System;
using UnityEngine;

namespace Field
{
    public class Inventory : MonoBehaviour
    {
        public InventorySlot[] Slots = new  InventorySlot[9];
        private int _currentIndex = 0;

        public Action<int> OnSlotChanged = null;
        
        void ChangeSlot(int index)
        {
            _currentIndex = index;
            OnSlotChanged?.Invoke(_currentIndex);
        }
        
        /// <summary>
        /// 현재 들고있는 아이템 반환
        /// </summary>
        /// <returns></returns>
        public Item GetCurrentItem() => Slots[_currentIndex]?.Item;

        /// <summary>
        /// 인벤토리에 아이템 추가
        /// </summary>
        /// <param name="item">추가할 아이템</param>
        /// <param name="amount">추가할 아이템 개수</param>
        /// <returns>추가 성공 여부</returns>
        public bool AddItem(Item item, int amount =1)
        {
            foreach (var slot in Slots)
            {
                if (slot == null || slot.Item != item) continue;
                slot.Amount++;
                return true;

            }

            for (var i = 0; i < Slots.Length; i++)
            {
                if (Slots[i] != null && Slots[i].Item != null) continue;
                Slots[i] = new InventorySlot(item, amount);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 인벤토리에 아이템 제거
        /// </summary>
        /// <param name="item">제거할 아이템</param>
        /// <param name="amount">제거할 개수</param>
        public void RemoveItem(Item item, int amount = 1)
        {
            var slot = Array.Find(Slots, x => x.Item == item);
            if (slot != null)
            {
                slot.Amount -= amount;
                if (slot.Amount <= 0) slot.Item = null;
            }
        }
    }

    public class InventorySlot
    {
        public Item Item;
        public int Amount;
        public  InventorySlot(Item item, int amount)
        {
            Item = item;
            Amount = amount;
        }
        
    }
}
