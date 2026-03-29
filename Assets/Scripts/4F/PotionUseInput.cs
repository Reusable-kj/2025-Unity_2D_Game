// PotionUseInput.cs
using UnityEngine;

public class PotionUseInput : MonoBehaviour
{
    [SerializeField] private KeyCode usePotionKey = KeyCode.Alpha2; // 포션 사용 키 (기본: 숫자 2)

    void Update()
    {
        if (!Input.GetKeyDown(usePotionKey)) return;
        if (PlayerInventory.Instance == null) return;

        if (PlayerInventory.Instance.hasPotion)
        {
            PlayerInventory.Instance.RemovePotion();
        }
        else
        {
            Debug.Log("PotionUseInput: 포션이 없습니다.");
        }
    }
}
