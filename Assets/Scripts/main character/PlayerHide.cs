using UnityEngine;

public class PlayerHide : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private string interactTag = "InteractOBJ"; // 캐비닛 등에 붙인 태그
    [SerializeField] private KeyCode hideKey = KeyCode.F;

    // 외부에서 읽기 전용으로 확인 가능
    public bool isHiding { get; private set; }

    // 내부 상태
    private Collider2D currentCollider;       // 들어간 캐비닛(트리거) 참조
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D[] myColliders;
    private MonoBehaviour movement;           // 이동 스크립트(이름이 Movement가 아닐 수도 있어서 범용)

    // 원래 값 저장용
    private float origGravity;
    private RigidbodyConstraints2D origConstraints;
    private int origLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        myColliders = GetComponentsInChildren<Collider2D>(true);
        // 이동 스크립트가 Movement 라면 이렇게:
        movement = GetComponent<Movement>();
        // 다르면 GetComponent<당신의이동스크립트이름>(); 로 바꾸세요.
    }

    private void Update()
    {
        if (Input.GetKeyDown(hideKey) && currentCollider != null)
        {
            ToggleHide();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(interactTag))
        {
            currentCollider = other;
            //Debug.Log("상호작용 가능: " + other.name);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == currentCollider && !isHiding)
        {
            currentCollider = null;
            //Debug.Log("상호작용 범위 벗어남");
        }
    }

    public void ToggleHide()
    {
        if (!isHiding)
        {
            // 들어가기
            isHiding = true;

            // 원래 물리/레이어 저장
            origGravity = rb.gravityScale;
            origConstraints = rb.constraints;
            origLayer = gameObject.layer;

            // 물리 정지 & 위치 이동(캐비닛 중앙)
            rb.linearVelocity = Vector2.zero;
            transform.position = currentCollider.transform.position;

            // ★ 물리 완전 차단: 시뮬레이션/콜라이더 비활성화
            rb.simulated = false;
            foreach (var c in myColliders) c.enabled = false;

            // 시각/조작 비활성화
            if (sr) sr.enabled = false;
            if (movement) movement.enabled = false;

            // 필요하면 숨김 전용 레이어로 변경(옵션)
            // gameObject.layer = LayerMask.NameToLayer("Hidden");

            //Debug.Log("숨기 시작");
        }
        else
        {
            // 나오기
            isHiding = false;

            // 물리 복구
            foreach (var c in myColliders) c.enabled = true;
            rb.simulated = true;
            rb.gravityScale = origGravity;
            rb.constraints = origConstraints;
            gameObject.layer = origLayer;

            // 시각/조작 복구
            if (sr) sr.enabled = true;
            if (movement) movement.enabled = true;

            // 캐비닛 트리거 범위를 아직 벗어나지 않았다면 계속 F로 다시 들어갈 수 있게 유지
            //Debug.Log("숨기 종료");
        }
    }
}