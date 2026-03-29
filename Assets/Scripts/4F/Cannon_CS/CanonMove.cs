using UnityEngine;

public class CanonMove : MonoBehaviour
{
    // === 인스펙터 설정 변수 ===
    [Header("제어대상")]
    private Movement playerMovement;      // 플레이어 움직임 스크립트
    private bool isPlayerNear = false;    // 플레이어 근접 여부
    
    [Header("이동 및 고정 설정")]
    public float followSpeed = 5f;               // 플레이어를 따라가는 속도
    public float maxDistanceToPlayer = 1.0f;     // 플레이어와 대포 간 최대 거리
    public KeyCode moveToggleKey = KeyCode.E;    // 이동/고정 토글 키
    
    [Header("발사 설정")]
    public KeyCode fireKey = KeyCode.F;          // 발사 키
    public Transform firePoint;                  // 포탄이 나갈 위치
    public GameObject projectilePrefab;          // 포탄 프리팹
    public KeyCode AimToggleKey = KeyCode.X;     // (현재 사용 안 함)

    [Header("사운드 설정")]
    public AudioSource audioSource;              // 대포 오브젝트에 붙은 AudioSource
    public AudioClip fireClip;                   // 포탄 발사 효과음

    // === 상태 변수 ===
    private bool isFollowingPlayer = false;      // 플레이어 따라가는 중인지
    public bool HasMortarRound { get; private set; } = false; // 포탄을 들고 있는지
    private Transform playerTransform;           // 플레이어 Transform

    void Start()
    {
        // Movement 스크립트 & Transform 찾기
        playerMovement = FindObjectOfType<Movement>();
        if (playerMovement != null)
            playerTransform = playerMovement.transform;

        // Rigidbody2D 있으면 물리 힘 안 받도록
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // 시작 시 BOOM UI는 꺼진 상태 보장
        if (BoomUI.Instance != null)
        {
            BoomUI.Instance.SetBoom(false);
        }

        // AudioSource 자동으로 가져오기 (인스펙터에서 안 넣어도 되게)
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    // === 포탄 획득/소모 함수 (외부에서 호출) ===
    public void AcquireMortarRound()
    {
        HasMortarRound = true;
        Debug.Log("포탄 획득! 발사 준비 완료.");

        // 🔥 BOOM 아이콘 켜기
        if (BoomUI.Instance != null)
        {
            BoomUI.Instance.SetBoom(true);
        }
        else
        {
            Debug.LogWarning("CanonMove: BoomUI.Instance 가 없습니다. BOOM 아이콘을 켤 수 없음.");
        }
        Debug.Log("[CanonMove] 포탄 획득! 발사 가능 상태.");
    }

    public void ConsumeMortarRound()
    {
        HasMortarRound = false;
        Debug.Log("포탄 소모. 포탄 재장전 필요.");

        // 🔥 BOOM 아이콘 끄기
        if (BoomUI.Instance != null)
        {
            BoomUI.Instance.SetBoom(false);
        }
        Debug.Log("[CanonMove] 포탄 소모. 재장전 필요.");
    }

    void Update()
    {
        if (playerTransform == null) return;

        // A. 이동/고정 전환 (E 키)
        if (Input.GetKeyDown(moveToggleKey))
        {
            if (isPlayerNear)
                isFollowingPlayer = !isFollowingPlayer;
        }

        // B. 상태별 처리
        if (isFollowingPlayer)
        {
            FollowPlayer();
        }
        else
        {
            FireCannon();
        }
    }

    // 플레이어 따라가기
    void FollowPlayer()
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance > maxDistanceToPlayer)
        {
            Vector3 targetPos = playerTransform.position;
            targetPos.y = transform.position.y; // 높이는 고정
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);
        }
    }

    // 고정 상태에서 포탄 발사
    void FireCannon()
    {
        // 고정 상태 + 포탄 소지 + 발사키 눌렀을 때만
        if (HasMortarRound && Input.GetKeyDown(fireKey))
        {
            PerformLaunch();
            ConsumeMortarRound();
        }
    }

    // 실제 발사 처리 + 사운드 재생
    void PerformLaunch()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogError("[CanonMove] 발사 실패: projectilePrefab 또는 firePoint 미지정!");
            return;
        }

        // 1) 포탄 생성
        GameObject bomb = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // 2) 포탄 이동 시작
        BombProjectile bombScript = bomb.GetComponent<BombProjectile>();
        if (bombScript != null)
        {
            bombScript.isLaunched = true;
        }

        // 3) 발사 사운드 재생
        if (audioSource != null && fireClip != null)
        {
            audioSource.PlayOneShot(fireClip);
        }
        else
        {
            Debug.LogWarning("[CanonMove] 발사 사운드를 재생할 수 없음 (audioSource 또는 fireClip 미할당)");
        }

        Debug.Log("[CanonMove] 포탄 발사!");
    }

    // 플레이어 근접 여부 처리
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            Debug.Log("[CanonMove] 플레이어 근접: E키로 대포 이동/고정 토글 가능.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            Debug.Log("[CanonMove] 플레이어 이탈: 대포 조작 불가.");
        }
    }

    public void ForceStopAiming()
    {
        // 필요하면 나중에 조준 관련 기능 추가
        Debug.Log("[CanonMove] ForceStopAiming 호출됨 (현재는 로그만)");
    }
}
