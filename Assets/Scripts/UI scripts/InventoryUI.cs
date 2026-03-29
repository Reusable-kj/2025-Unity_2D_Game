using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject keyIcon; // 열쇠 아이콘 UI 오브젝트

    private void Start()
    {
        if (PlayerInventory.Instance == null)
        {
            Debug.LogWarning("PlayerInventory Instance 없음");
            return;
        }

        // 처음 상태 반영
        UpdateKeyIcon(PlayerInventory.Instance.HasKey);

        // 키 변화 이벤트 구독
        PlayerInventory.Instance.OnKeyChanged += UpdateKeyIcon;
    }

    private void OnDestroy()
    {
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnKeyChanged -= UpdateKeyIcon;
        }
    }

    private void UpdateKeyIcon(bool hasKey)
    {
        if (keyIcon != null)
            keyIcon.SetActive(hasKey);
    }
}
