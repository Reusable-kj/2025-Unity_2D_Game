// HealthUI.cs
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image healthImage;      // 체력 바 이미지 (하나)

    [Header("체력 상태별 스프라이트")]
    [SerializeField] private Sprite health3Sprite;   // Health_3
    [SerializeField] private Sprite health2Sprite;   // Health_2
    [SerializeField] private Sprite health1Sprite;   // Health_1

    private int lastHp = -1; // 마지막으로 표시한 체력값 (변화 체크용)

    private void Awake()
    {
        if (healthImage == null)
            healthImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (PlayerInventory.Instance == null || healthImage == null)
            return;

        int hp = PlayerInventory.Instance.Playerheart;

        // 체력이 바뀐 경우에만 이미지 갱신
        if (hp == lastHp) return;
        lastHp = hp;

        Refresh(hp);
    }

    private void Refresh(int hp)
    {
        if (hp >= 3)
        {
            healthImage.sprite = health3Sprite;
            healthImage.enabled = true;
        }
        else if (hp == 2)
        {
            healthImage.sprite = health2Sprite;
            healthImage.enabled = true;
        }
        else if (hp == 1)
        {
            healthImage.sprite = health1Sprite;
            healthImage.enabled = true;
        }
        else
        {
            // hp가 0 이하 → 이미지 숨기기
            healthImage.enabled = false;
        }
    }
}
