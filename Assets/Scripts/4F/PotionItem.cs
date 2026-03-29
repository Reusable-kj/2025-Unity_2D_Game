// PotionItem.cs
using UnityEngine;
using System.Collections; // 코루틴 사용을 위해 필요

public class PotionItem : MonoBehaviour
{
    // === 인스펙터 설정 ===
    [Header("F키 누르고 있어야 하는 시간")]
    public float holdTime = 1f;

    // === 내부 관리 변수 ===
    public PlayerInventory inventory; // PlayerInventory.Instance를 통해 참조됨

    private bool isPlayerInside = false; // 플레이어가 콜라이더 안에 있는지
    private bool isHolding = false;      // F키를 누르고 있는지 (Coroutine 상태 플래그)

    void Start()
    {
        // 씬 로드 타이밍과 무관하게 PlayerInventory 싱글톤 인스턴스를 참조합니다.
        inventory = PlayerInventory.Instance;

        if (inventory == null)
        {
            Debug.LogError("PotionItem: PlayerInventory 싱글톤 인스턴스를 찾을 수 없습니다. 설정 확인 필요.");
        }
    }

    void Update()
    {
        // F키를 누를 때, 범위 안에 있고(isPlayerInside), 이미 사용 중이 아닐 때(isHolding)
        if (isPlayerInside && Input.GetKeyDown(KeyCode.F) && !isHolding)
        {
            isHolding = true;
            StartCoroutine(HoldToUseItem());
        }
    }

    // 플레이어가 포션 영역에 진입했을 때
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            Debug.Log("PotionItem: 플레이어 감지");
        }
    }

    // 플레이어가 포션 영역을 벗어났을 때
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            Debug.Log("PotionItem: 플레이어 이탈");

            // F키를 떼거나 영역을 벗어나면 코루틴 정지
            if (isHolding)
            {
                StopAllCoroutines();
                isHolding = false;
            }
        }
    }

    // F키 홀드 처리
    IEnumerator HoldToUseItem()
    {
        float timer = 0f;

        while (timer < holdTime)
        {
            // F키를 누르고 있어야 하고, 플레이어가 범위 안에 있어야 타이머 증가
            if (Input.GetKey(KeyCode.F) && isPlayerInside)
            {
                timer += Time.deltaTime;
            }
            else
            {
                // F키를 떼거나 영역을 벗어나면 코루틴 중단 및 타이머 리셋
                isHolding = false;
                yield break;
            }
            yield return null;
        }

        // ⭐ 홀드 시간 충족: 포션 획득 로직
        if (inventory != null)
        {
            // ✅ PlayerInventory의 AddPotion()을 통해 처리
            inventory.AddPotion();
            Debug.Log("PotionItem: 포션 획득 완료! hasPotion = " + inventory.hasPotion);
        }
        else
        {
            Debug.LogWarning("PotionItem: inventory 가 null 입니다.");
        }

        // 포션 오브젝트 파괴 (획득 완료)
        Destroy(gameObject);
    }
}
