using UnityEngine;

public class PlayerFeet : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerHide playerHide;
    private PlayerCollision playerCollision;

    [Header("Stomp Conditions")]
    [Tooltip("이 값보다 더 음수(vy < minFallSpeed)일 때만 낙하로 인정")]
    [SerializeField] private float minFallSpeed = -0.2f;

    [Header("Head Tag 사용 여부(선택)")]
    [SerializeField] private bool requireHeadTag = false;
    [SerializeField] private string enemyHeadTag = "EnemyHead";

    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        playerHide = GetComponentInParent<PlayerHide>();
        playerCollision = GetComponentInParent<PlayerCollision>();
    }

    private void OnEnable()
    {
        var col = GetComponent<Collider2D>();
        if (col == null || !col.isTrigger)
            Debug.LogWarning("[Feet] 내 Collider2D 가 없거나 IsTrigger가 꺼져 있음!");
        if (rb == null)
            Debug.LogWarning("[Feet] 부모에 Rigidbody2D 없음!");
    }

    private void OnTriggerEnter2D(Collider2D other) => TryStomp(other, "Enter");
    private void OnTriggerStay2D  (Collider2D other) => TryStomp(other, "Stay");

    private void TryStomp(Collider2D other, string phase)
    {
        if (playerHide && playerHide.isHiding)
        {
            // 숨는 중이면 무시
            // Debug.Log($"[Feet:{phase}] 숨는 중 → 무시");
            return;
        }

        // (선택) Head 태그 강제
        if (requireHeadTag && !other.CompareTag(enemyHeadTag))
        {
            // Debug.Log($"[Feet:{phase}] HeadTag 불일치: {other.name} tag={other.tag}");
            return;
        }

        // 부모 체인 또는 자기 자신에서 StompableEnemy 찾기(둘 다 검사)
        var enemy = other.GetComponentInParent<StompableEnemy>();
        if (enemy == null) enemy = other.GetComponent<StompableEnemy>();
        if (enemy == null)
        {
            // Debug.Log($"[Feet:{phase}] StompableEnemy 없음: {other.name} tag={other.tag}");
            return;
        }

        if (rb == null)
        {
            Debug.LogWarning($"[Feet:{phase}] Rigidbody2D 없음");
            return;
        }

        // 낙하 중 판정 (위로 치솟는 중이면 밟기 불가)
        if (rb.linearVelocity.y >= minFallSpeed)
        {
            // Debug.Log($"[Feet:{phase}] 낙하 아님 (vy={rb.velocity.y:F2})");
            return;
        }

        // 여기까지 오면 성공
        if (playerCollision) playerCollision.SetStompGrace(0.25f); // 본체 충돌 무시 타임

        // 후속 충돌 방지: 적 콜라이더 비활성 → 파괴
        foreach (var c in enemy.GetComponentsInChildren<Collider2D>()) c.enabled = false;
        Destroy(enemy.gameObject);

        Debug.Log($"[Feet:{phase}] ✅ 밟아서 제거 (target={enemy.name}, vy={rb.linearVelocity.y:F2})");
    }
}