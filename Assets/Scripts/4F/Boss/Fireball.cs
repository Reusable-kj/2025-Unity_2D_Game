using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("설정")]
    public float speed = 7f;           // 날아가는 속도
    public GameObject trapPrefab;      // 바닥에 닿으면 생성될 잔상(트랩) 프리팹
    public float lifeTime = 7f;        // 5초 동안 아무데도 안 닿으면 자동 삭제

    private Vector3 targetPosition;
    private Vector3 moveDirection;
    private bool isLaunched = false;
    
    private DamageReceiver damageReceiver;
    // BossController에서 호출하여 방향을 설정함
    public void Launch(Vector3 targetPos)
    {
        targetPosition = targetPos;
        moveDirection = (targetPosition - transform.position).normalized;
        isLaunched = true;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (!isLaunched) return;

        // 1. 목표 좌표를 향해 이동 (MoveTowards는 물리 연산을 무시하고 Transform을 이동)
        transform.position += moveDirection * speed * Time.deltaTime;
        
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            SpawnTrap(); // 잔상 생성
            Destroy(gameObject); // 임무 완료 후 불꽃 파괴
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 1. 플레이어와 직접 충돌 (즉시 데미지)
        if (other.CompareTag("Player"))
        {
            
            DamageReceiver receiver = other.GetComponent<DamageReceiver>();
            
            if (receiver != null)
            {
                // DamageReceiver를 통해 데미지를 전달합니다.
                // receiver가 무적 상태 확인, HP 감소, 무적 시간 시작을 모두 처리합니다.
                receiver.TakeDamage(1); // 파이어볼의 기본 데미지(1)를 직접 지정하거나 public 변수를 사용하세요.
                Debug.Log("플레이어 불꽃 직격! 데미지 처리 완료.");
            }
            
            Destroy(gameObject); // 데미지 처리 후 불꽃 파괴
            return; // 이후 로직 실행 방지
        }
        if(other.gameObject.name == "MapBoundary")
        {
            SpawnTrap();
            Destroy(gameObject, lifeTime);
        }
    }

    void SpawnTrap()
    {
        if (trapPrefab != null)
        {
            // 목표 지점에 FireTrap 생성
            GameObject trapInstance = Instantiate(trapPrefab, targetPosition, Quaternion.identity);
        
            // ⭐ [핵심 추가] 생성 직후 trapInstance를 3초 뒤에 파괴하도록 명령합니다. ⭐
            Destroy(trapInstance, 3f);
        }
    }
}