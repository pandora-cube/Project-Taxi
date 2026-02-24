using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Field
{
    public class InventoryView : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset slotAsset; // Slot.uxml
        [SerializeField] private Inventory inventoryData; // 위에서 작성하신 Inventory 클래스

        private VisualElement root;
        private VisualElement container;

        // 생성된 9개의 슬롯 UI 요소를 보관할 리스트 (인덱스 접근용)
        private List<VisualElement> slotUIList = new List<VisualElement>();

        void OnEnable()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            // Inventory.uxml 안에 슬롯들이 들어갈 부모 요소 (이름: SlotContainer)
            container = root.Q<VisualElement>("SlotContainer");

            InitializeInventory();

            // 데이터 변경 이벤트 구독
            inventoryData.OnSlotChanged += UpdateSlotUI;
        }

        void OnDisable()
        {
            // 이벤트 구독 해제 (메모리 누수 방지)
            if (inventoryData != null)
                inventoryData.OnSlotChanged -= UpdateSlotUI;
        }

        // 1. 처음에 9개의 빈 슬롯 UI를 미리 생성
        private void InitializeInventory()
        {
            container.Clear();
            slotUIList.Clear();

            for (int i = 0; i < inventoryData.Slots.Length; i++)
            {
                VisualElement slotInstance = slotAsset.Instantiate();
                container.Add(slotInstance);
                slotUIList.Add(slotInstance);

                // 초기 상태 업데이트
                UpdateSlotUI(i);
            }
        }

        // 2. 특정 인덱스의 데이터가 바뀌었을 때 실행될 함수
        private void UpdateSlotUI(int index)
        {
            Debug.Log(index);
            if (index < 0 || index >= slotUIList.Count) return;

            var slotData = inventoryData.Slots[index];
            var slotUI = slotUIList[index];

            // Slot.uxml 내부의 요소들 찾기
            var icon = slotUI.Q<VisualElement>("Icon"); // 아이콘 표시용 VisualElement
            var amountLabel = slotUI.Q<Label>("Text"); // 개수 표시용 Label

            if (slotData != null && slotData.Item != null)
            {
                // 아이템이 있는 경우
                amountLabel.text = slotData.Item.ItemName + "/" + slotData.Amount;
                icon.style.display = DisplayStyle.Flex;
            }
            else
            {
                // 아이템이 없는 경우 (빈 슬롯)
                icon.style.backgroundImage = null;
                amountLabel.text = "0";
                icon.style.display = DisplayStyle.None; // 혹은 투명하게 처리
            }
        }
    }
}
