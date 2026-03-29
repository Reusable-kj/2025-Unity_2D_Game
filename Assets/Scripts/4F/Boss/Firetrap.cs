using UnityEngine;

public class FireTrap : MonoBehaviour
{
    [Header("트랩 설정")]
    public float duration = 3.0f;       // 트랩 유지 시간 (자동 파괴)
    public int damageAmount = 1;        // 트랩이 주는 피해량

    void Start()
    {
        // 일정 시간 후 자동으로 파괴 (메모리 관리)
        Destroy(gameObject, duration);
    }

    // 플레이어가 트랩 범위에 진입했을 때만 호출됩니다. (1회 피해 로직)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Damage Receiver를 찾아서 피해를 전달합니다.
            DamageReceiver receiver = other.GetComponent<DamageReceiver>();
            
            if (receiver != null)
            {
                // receiver가 무적 상태를 체크하고, 피해를 적용하며, 무적 시간을 시작합니다.
                receiver.TakeDamage(damageAmount);
                Debug.Log("잔상 트랩 1회 피해 처리 완료. 무적 시간 시작.");
            }
        }
    }
}