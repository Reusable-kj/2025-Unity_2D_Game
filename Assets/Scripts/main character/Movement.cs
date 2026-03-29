using UnityEngine;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    private bool inputLocked = false; //조작잠금
    public static Vector3 RestartSpawnPoint = new Vector3(-21f, -3f, 0f); // 1층 스폰 좌표로 설정
    public static bool IsRestarting = false;
    
    [Header("Settings")]
    public float JumpForce;
    public float MaxSpeed;
    public float MoveForce;

    [Header("References")]
    public Rigidbody2D PlayerRigidBody;
    public Animator animator;

    private bool onGround = true;

        // 성능을 위해 해시 캐싱
    private static readonly int StateHash = Animator.StringToHash("state");

        // 걷기/정지 판정 임계값(너무 민감하지 않게 약간의 여유)
    private const float WalkThreshold = 0.1f;
    
    void Awake()
    {
        // 씬 로드 이벤트 구독 
        // 플레이어 오브젝트가 DDOL이므로, 이벤트를 구독하여 씬 변경을 감지합니다.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void Update()
    {
        if (inputLocked) return; //조작 잠금이 해제시 동작
        
        float moveInput = Input.GetAxisRaw("Horizontal");

        // 좌우 이동
        if (Mathf.Abs(moveInput) > 0f)
        {
            // ✅ velocity 로 수정
            if (Mathf.Abs(PlayerRigidBody.linearVelocity.x) < MaxSpeed)
            {
                PlayerRigidBody.AddForce(new Vector2(moveInput * MoveForce, 0), ForceMode2D.Impulse);
            }

            // 방향 전환 시 속도 초기화
            if (Mathf.Sign(PlayerRigidBody.linearVelocity.x) != Mathf.Sign(moveInput))
            {
                PlayerRigidBody.linearVelocity = new Vector2(0, PlayerRigidBody.linearVelocity.y);
            }
        }

        // 좌우 반전
        if (moveInput != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(moveInput) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        // 점프
        if (Input.GetKeyDown(KeyCode.W) && onGround)
        {
            PlayerRigidBody.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            onGround = false;

            animator.SetInteger("state", 2);   // 점프 시작
        }

        if (!onGround)
        {
            animator.SetInteger("state", 2); // Jump
        }
        else
        {
            // 지면에 있을 때: 속도로 정지/이동 구분
            float speedX = Mathf.Abs(PlayerRigidBody.linearVelocity.x);
            animator.SetInteger("state", speedX > WalkThreshold ? 1 : 0); // Walk or Stop
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = true;

                     // 착지 직후에도 속도 보고 Walk/Stop 결정
            float speedX = Mathf.Abs(PlayerRigidBody.linearVelocity.x);
            animator.SetInteger("state", speedX > WalkThreshold ? 1 : 0);
        }
    }
    
    //아래 함수 두개는 외부호출이 가능한 조작잠금 플래그조정 함수임
    public void LockMovement()
    {
        inputLocked = true;
        if (GetComponent<Rigidbody2D>() != null)
        {
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
        animator.SetInteger("state", 0);
    }

    public void UnlockMovement()
    {
        inputLocked = false;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. 재시작 플래그와 로드된 씬 이름 확인
        if (IsRestarting && scene.name == "Floor_1") // ⭐ "Floor_1"을 실제 1층 씬 이름으로 변경 ⭐
        {
            // 2. ⭐ 플레이어 자신의 Transform 위치를 강제로 초기화 ⭐
            transform.position = RestartSpawnPoint;
        
            // 3. Rigidbody 속도 초기화 (이전 층의 관성 제거)
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

            // 4. 플래그 해제 (다음번 이동을 위해 리셋)
            IsRestarting = false; 
        
            Debug.Log($"[Restart Logic] Movement 스크립트가 플레이어({gameObject.name})를 재시작 위치로 이동 완료.");
        }
    }
}
