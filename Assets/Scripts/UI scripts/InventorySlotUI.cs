// InventorySlotUI.cs
using UnityEngine;

public class InventorySlotUI : MonoBehaviour
{
    public static InventorySlotUI Instance;

    [Header("아이템 아이콘들 (Inspector에서 비워두면 이름으로 자동 찾음)")]
    [SerializeField] private RectTransform keyIcon;        // "Key"
    [SerializeField] private RectTransform healPotionIcon; // "Heal_posion"

    [Header("슬롯 위치들 (먹은 순서대로 1,2,3 슬롯)")]
    [SerializeField] private Vector2[] slotPositions = new Vector2[3];

    // 🔹 각 슬롯에 어떤 아이템이 들어있는지 기록 (없으면 null)
    private InventoryItemType?[] slotItems;

    private void Awake()
    {
        // 싱글톤
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 슬롯 상태 배열 초기화
        slotItems = new InventoryItemType?[slotPositions.Length];

        // Inspector에서 안 넣었을 경우, 이름으로 자동 찾기
        if (keyIcon == null)
        {
            GameObject go = GameObject.Find("Key");
            if (go != null) keyIcon = go.GetComponent<RectTransform>();
        }

        if (healPotionIcon == null)
        {
            GameObject go = GameObject.Find("Heal_posion");
            if (go != null) healPotionIcon = go.GetComponent<RectTransform>();
        }
    }

    private void Start()
    {
        // 처음에는 전부 안 보이게
        if (keyIcon != null) keyIcon.gameObject.SetActive(false);
        if (healPotionIcon != null) healPotionIcon.gameObject.SetActive(false);
    }

    /// <summary>
    /// type 아이템을 인벤토리에 추가.
    /// → "첫 번째 비어있는 슬롯"을 찾아서 거기에 배치.
    /// </summary>
    public void AddItem(InventoryItemType type)
    {
        if (slotItems == null || slotItems.Length == 0)
        {
            Debug.LogWarning("InventorySlotUI: slotItems 배열이 초기화되지 않았습니다.");
            return;
        }

        // 🔸 이미 같은 타입 아이템이 인벤토리에 있다면, 중복 추가 방지
        for (int i = 0; i < slotItems.Length; i++)
        {
            if (slotItems[i].HasValue && slotItems[i].Value == type)
            {
                Debug.Log($"InventorySlotUI: 이미 인벤토리에 있는 아이템입니다: {type}");
                return;
            }
        }

        // 🔸 첫 번째 빈 슬롯 찾기
        int freeIndex = -1;
        for (int i = 0; i < slotItems.Length; i++)
        {
            if (!slotItems[i].HasValue)
            {
                freeIndex = i;
                break;
            }
        }

        if (freeIndex == -1)
        {
            Debug.LogWarning("InventorySlotUI: 인벤토리 슬롯이 가득 찼습니다.");
            return;
        }

        // 해당 타입에 맞는 아이콘 가져오기
        RectTransform icon = GetIconByType(type);
        if (icon == null)
        {
            Debug.LogWarning("InventorySlotUI: 아이콘 RectTransform이 설정되지 않았습니다: " + type);
            return;
        }

        // 아이콘 활성화 + 슬롯 위치로 이동
        icon.gameObject.SetActive(true);
        icon.anchoredPosition = slotPositions[freeIndex];

        // 슬롯 상태 기록
        slotItems[freeIndex] = type;

        Debug.Log($"InventorySlotUI: {type} 아이템이 {freeIndex}번 슬롯에 배치됨");
    }

    /// <summary>
    /// 아이템을 사용/삭제할 때 호출.
    /// 해당 타입이 들어 있던 슬롯을 찾아 비우고, 아이콘을 끔.
    /// </summary>
    public void RemoveItem(InventoryItemType type)
    {
        if (slotItems == null) return;

        // 🔸 이 타입이 들어있는 슬롯 찾기
        int indexToClear = -1;
        for (int i = 0; i < slotItems.Length; i++)
        {
            if (slotItems[i].HasValue && slotItems[i].Value == type)
            {
                indexToClear = i;
                break;
            }
        }

        if (indexToClear == -1)
        {
            // 이 타입 아이템이 인벤토리에 없음
            Debug.Log($"InventorySlotUI: {type} 아이템이 슬롯에 없습니다.");
            return;
        }

        // 아이콘 비활성화
        RectTransform icon = GetIconByType(type);
        if (icon != null)
            icon.gameObject.SetActive(false);

        // 슬롯 상태 비우기
        slotItems[indexToClear] = null;

        Debug.Log($"InventorySlotUI: {type} 아이템이 {indexToClear}번 슬롯에서 제거됨");
    }

    /// <summary>
    /// 게임오버 등 전체 리셋할 때 사용.
    /// </summary>
    public void ResetAllSlots()
    {
        if (slotItems != null)
        {
            for (int i = 0; i < slotItems.Length; i++)
            {
                slotItems[i] = null;
            }
        }

        if (keyIcon != null) keyIcon.gameObject.SetActive(false);
        if (healPotionIcon != null) healPotionIcon.gameObject.SetActive(false);

        Debug.Log("InventorySlotUI: 모든 슬롯 리셋");
    }

    // 타입에 맞는 아이콘 반환하는 헬퍼 함수
    private RectTransform GetIconByType(InventoryItemType type)
    {
        switch (type)
        {
            case InventoryItemType.Key:
                return keyIcon;
            case InventoryItemType.HealPotion:
                return healPotionIcon;
        }
        return null;
    }
}
