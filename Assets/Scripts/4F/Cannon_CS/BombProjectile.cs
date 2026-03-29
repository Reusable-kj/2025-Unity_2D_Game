using UnityEngine;

public class BombProjectile : MonoBehaviour
{
    [Header("폭발 연출")]
    public Sprite explosionSprite; // 폭발 이미지 스프라이트
    public float explosionDuration = 1f; // 폭발 이미지를 보여줄 시간 (1초)
    private SpriteRenderer sr; // 스프라이트 렌더러 컴포넌트 참조
    private int damage = 1;
    
    [Header("이동 및 파괴 설정")]
    public float moveSpeed = 10f; // 포탄의 수직 이동 속도
    public float ceilingYLimit = 50f; // 맵 천장(파괴될 Y좌표)
    [Header("폭발 크기")]
    public float explosionScaleFactor = 3f; // 기본 3배 크기로 설정
    
    // 이 플래그가 true일 때만 포탄이 움직이게 됩니다.
    [HideInInspector] public bool isLaunched = false; 
    
    // [TODO] damageAmount 변수 추가 (예: public int damageAmount = 1;)

    void Awake()
    {
        // 스프라이트 렌더러 컴포넌트를 찾습니다.
        sr = GetComponent<SpriteRenderer>();
    }
    
    void Start()
    {
        // 포탄의 Rigidbody2D를 Kinematic으로 설정하여 물리 시스템을 무시하도록 합니다.
        // (충돌 감지는 Collider2D가 담당)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true; 
        }
    }
    
    void Update()
    {
        if (!isLaunched) return; // 발사 명령이 내려지지 않았다면 정지

        // ⭐ 1. Y축으로만 Transform을 직접 이동시킵니다. ⭐
        transform.position += (Vector3.up * moveSpeed * Time.deltaTime);

        // 2. 천장 도달 시 자동 파괴 (메모리 정리)
        if (transform.position.y > ceilingYLimit)
        {
            Debug.Log("포탄 천장 도달. 자동 파괴.");
            Destroy(gameObject);
        }
    }
    
    // ⭐ 3. 보스 충돌 감지 ⭐
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Boss"))
        {
            BossController bossController = other.gameObject.GetComponent<BossController>();
            if (bossController != null)
            {
                bossController.TakeDamage(damage);
                Debug.Log("보스 명중!");

                Explode();
                return;
            }
        }
    }
    void Explode()
    {
        // 1. 이동 정지 명령 (Update 함수가 더 이상 실행되지 않게 하거나, 스피드를 0으로 만듭니다.)
        isLaunched = false;
    
        // 2. ⭐ [핵심] 스프라이트를 폭발 이미지로 교체합니다. ⭐
        if (sr != null && explosionSprite != null)
        {
            sr.sprite = explosionSprite;
        }
        transform.localScale = Vector3.one * explosionScaleFactor;
        // 3. 충돌체와 Rigidbody를 비활성화하여 추가적인 물리적 상호작용을 막습니다.
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;
    
        // 4. 폭발 Duration 이후 오브젝트를 파괴합니다.
        Destroy(gameObject, explosionDuration); 
    }
}