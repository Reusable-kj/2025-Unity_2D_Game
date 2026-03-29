// PlayerInventory.cs
using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // ★ 싱글톤 인스턴스
    public static PlayerInventory Instance;

    // 체력 (0~3칸이라고 가정)
    public int Playerheart = 3;

    // 포션 소지 여부 (힐 포션)
    public bool hasPotion = false;

    // 키 관련 이벤트 (InventoryUI 등에서 사용 가능)
    public event Action<bool> OnKeyChanged;

    [SerializeField] private bool _hasKey;
    public bool HasKey
    {
        get => _hasKey;
        private set
        {
            if (_hasKey == value) return;
            _hasKey = value;
            OnKeyChanged?.Invoke(_hasKey);
        }
    }

    private void Awake()
    {
        // ★ 싱글톤 + DontDestroyOnLoad
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);   // 플레이어(인벤토리 포함) 유지
    }

    // ======================
    // 🔹 포션 관련 메서드
    // ======================
    public void AddPotion()
    {
        // 이미 한 개 들고 있으면 또 추가 안 함 (원하면 이 if 문 지워도 됨)
        if (hasPotion) return;

        hasPotion = true;

        // 슬롯 인벤토리 UI에도 HealPotion 아이콘 추가
        if (InventorySlotUI.Instance != null)
        {
            InventorySlotUI.Instance.AddItem(InventoryItemType.HealPotion);
        }
        else
        {
            Debug.LogWarning("AddPotion: InventorySlotUI.Instance 가 null 입니다.");
        }

        Debug.Log("PlayerInventory: HealPotion 획득, hasPotion = true");
    }

    public void RemovePotion()
    {
        if (!hasPotion) return;

        hasPotion = false;

        // 🔹 1) 체력 1칸 회복 (최대 3칸이라고 가정)
        Playerheart = Mathf.Min(Playerheart + 1, 3);

        // 🔹 2) 인벤토리 슬롯 UI에서 HealPotion 아이콘 제거
        if (InventorySlotUI.Instance != null)
        {
            InventorySlotUI.Instance.RemoveItem(InventoryItemType.HealPotion);
        }
        else
        {
            Debug.LogWarning("RemovePotion: InventorySlotUI.Instance 가 null 입니다.");
        }

        Debug.Log($"PlayerInventory: HealPotion 사용, 체력 = {Playerheart}");
    }

    // ======================
    // 🔹 키 관련 메서드
    // ======================

    // 키 줍기 + 슬롯 UI 반영
    public void GiveKey()
    {
        // 이미 키 있으면 중복으로 안 주도록
        if (HasKey) return;

        HasKey = true;   // 논리 상태 변경 + OnKeyChanged 이벤트 발동

        // 인벤토리 슬롯 UI에도 "키 하나 추가" 알리기
        if (InventorySlotUI.Instance != null)
        {
            InventorySlotUI.Instance.AddItem(InventoryItemType.Key);
        }

        Debug.Log("PlayerInventory: Key 획득");
    }

    // 키 사용 (성공 시 true)
    public bool UseKeyIfHas()
    {
        if (!HasKey) return false;

        // 1) 논리상 키 제거 + OnKeyChanged(false)
        HasKey = false;

        // 2) 슬롯 UI에서도 키 아이콘 제거
        if (InventorySlotUI.Instance != null)
        {
            InventorySlotUI.Instance.RemoveItem(InventoryItemType.Key);
        }

        Debug.Log("PlayerInventory: Key 사용");
        return true;
    }

    // (기존 코드 호환용) hasKey 프로퍼티
    public bool hasKey
    {
        get => HasKey;
        set => HasKey = value;
    }

    public void ResetKey()
    {
        // 논리 상태 리셋 + 상단 키 아이콘 끄기
        HasKey = false;   // OnKeyChanged(false) 호출

        // 슬롯 인벤토리에서도 전부 리셋
        if (InventorySlotUI.Instance != null)
        {
            InventorySlotUI.Instance.ResetAllSlots();
        }

        Debug.Log("PlayerInventory: 키/슬롯 리셋");
    }
}
