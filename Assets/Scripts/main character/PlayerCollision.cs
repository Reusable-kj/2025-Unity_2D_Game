using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI; //재시작 버튼을 사용하기 위해 추가

public class PlayerCollision : MonoBehaviour
{
    private PlayerInventory playerInventory;
    private PlayerHide playerHide;
    private Rigidbody2D rb;
    private Collider2D playerCollider;

    [Header("Damage Settings")]
    [SerializeField] public bool canTakeDamage = true;
    [SerializeField] private float damageCooldown = 1f;
    [SerializeField] private int blinkCount = 3;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject gameClearPanel;   // 인스펙터에 끌어다 연결
    [SerializeField] private string clearObjectTag = "ClearBox"; // 4F_box에 달 태그

    [Header("Monster Tag (Body collider)")]
    [SerializeField] private string monsterTag = "monster 1";

    // ✅ 하트 UI 애니메이터 연결
    [Header("Heart UI")]
    [SerializeField] private Animator heartAnimator;

    // 게임오버 패널
    [Header("UI References")]
    [SerializeField] private GameObject gameOverPanel;

    // ★ 맞았을 때 사운드
    [Header("Hit Sound")]
    [SerializeField] private AudioSource hitAudioSource; // 같은 오브젝트의 AudioSource
    [SerializeField] private AudioClip hitClip;          // 피격 효과음 클립

    // 밟은 직후 본체 충돌이 와도 무시하게 하는 짧은 타이머
    private float stompGraceTimer = 0f;

    private bool isCleared = false;

    private void Awake()
    {
        playerInventory = GetComponent<PlayerInventory>();
        playerHide      = GetComponent<PlayerHide>();
        rb              = GetComponent<Rigidbody2D>();
        playerCollider  = GetComponent<Collider2D>();
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();

        // ★ AudioSource 자동으로 찾아오기(비워두면)
        if (hitAudioSource == null)
            hitAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (!gameOverPanel)
            gameOverPanel = GameObject.Find("gameOverPanel");

        // ✅ 게임 시작 시 현재 체력으로 UI 초기화
        if (heartAnimator && playerInventory)
            heartAnimator.SetInteger("Health", playerInventory.Playerheart);
    }

    private void Update()
    {
        if (stompGraceTimer > 0f) stompGraceTimer -= Time.deltaTime;
    }

    /// <summary>Feet에서 ‘밟기’ 성공 시 호출: 잠깐 데미지 무시</summary>
    public void SetStompGrace(float seconds = 0.15f)
    {
        stompGraceTimer = Mathf.Max(stompGraceTimer, seconds);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 몬스터 '몸통'(Trigger 꺼짐)과의 실제 충돌만 데미지 처리
        if (!collision.gameObject.CompareTag(monsterTag)) return;

        // 숨어있거나, 방금 밟은 직후면 무시
        if ((playerHide && playerHide.isHiding) || stompGraceTimer > 0f) return;

        // 무적 타임 중이면 무시
        if (!canTakeDamage) return;

        if (playerInventory)
        {
            // ✅ 체력 감소
            playerInventory.Playerheart--;
            Debug.Log($"[PlayerCollision] 몬스터와 충돌! 남은 하트: {playerInventory.Playerheart}");

            // ★ 맞는 소리 재생
            PlayHitSound();

            // ✅ 애니메이터 파라미터 갱신 → 하트 UI 애니메이션 전환
            if (heartAnimator)
                heartAnimator.SetInteger("Health", playerInventory.Playerheart);

            if (playerInventory.Playerheart <= 0)
            {
                playerInventory.ResetKey();
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                if (gameOverPanel) gameOverPanel.SetActive(true); // 게임오버 패널 활성화
            }
            else
            {
                StartCoroutine(DamageCooldownAndInvincible(collision.collider));
            }
        }
    }

    // ★ 피격 사운드 재생 함수
    private void PlayHitSound()
    {
        if (hitAudioSource != null && hitClip != null)
        {
            hitAudioSource.PlayOneShot(hitClip);
        }
        else
        {
            Debug.LogWarning("[PlayerCollision] Hit sound 재생 실패 - AudioSource 또는 Clip이 비어 있음");
        }
    }

    public void RestartGame()
    {
        Debug.Log("RestartGame() 실행됨");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator DamageCooldownAndInvincible(Collider2D monsterCollider)
    {
        canTakeDamage = false;

        // 해당 몬스터 몸통과 잠시 충돌 무시
        if (playerCollider && monsterCollider)
            Physics2D.IgnoreCollision(playerCollider, monsterCollider, true);

        // 깜빡임
        if (spriteRenderer && blinkCount > 0 && damageCooldown > 0f)
        {
            float interval = damageCooldown / (blinkCount * 2f);
            for (int i = 0; i < blinkCount; i++)
            {
                SetAlpha(0.5f); yield return new WaitForSeconds(interval);
                SetAlpha(1f);   yield return new WaitForSeconds(interval);
            }
        }
        else if (damageCooldown > 0f)
        {
            yield return new WaitForSeconds(damageCooldown);
        }

        if (playerCollider && monsterCollider)
            Physics2D.IgnoreCollision(playerCollider, monsterCollider, false);

        canTakeDamage = true;
    }

    private void SetAlpha(float a)
    {
        if (!spriteRenderer) return;
        var c = spriteRenderer.color; c.a = a; spriteRenderer.color = c;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 이미 클리어 처리했다면 무시
        if (isCleared) return;

        // 4F_box에 "ClearBox" 태그를 달아두고 체크
        if (other.CompareTag(clearObjectTag) || other.name == "4F_box")
        {
            isCleared = true;

            // 패널 표시
            if (gameClearPanel) gameClearPanel.SetActive(true);

            Debug.Log("[PlayerCollision] Game Clear!");
        }
    }

    public void ResumeTime() { Time.timeScale = 1f; }

    public void LoadNextScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void StartDamageInvulnerability(Collider2D colliderToIgnore)
    {
        StartCoroutine(DamageCooldownAndInvincible(colliderToIgnore));
    }
}