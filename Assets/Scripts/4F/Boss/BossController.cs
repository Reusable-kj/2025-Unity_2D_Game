using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour
{
    [Header("보스 스탯")]
    private int maxHealth = 1; // 피격 5회 시 사망
    private int currentHealth;
    
    [Header("공격 설정")]
    public float attackInterval = 3.0f; // 3초마다 공격
    public GameObject firePrefab;       // 발사할 불꽃(Fireball) 프리팹
    public Transform firePoint;         // 불을 뿜는 위치 (보스 입)

    [Header("상태 확인")]
    public bool isDead = false;
    private Transform playerTransform;  // 플레이어 위치 추적용

    [Header("사운드")]
    public AudioSource audioSource;     // 보스 오브젝트에 붙은 AudioSource
    public AudioClip hitClip;           // 맞았을 때 나는 소리
    public AudioClip entranceRoarClip; // Inspector에서 보스 등장 시 포효 사운드 파일을 할당

    void Start()
    {
        currentHealth = maxHealth;

        // 플레이어 찾기 (DDOL로 넘어온 플레이어)
        Movement player = FindObjectOfType<Movement>();
        PlayerHealthWatcher playerHealthWatcher = FindObjectOfType<PlayerHealthWatcher>();
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // AudioSource 자동 할당 (인스펙터에 안 넣어도 되게)
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }
    public void BossRoarSound()
    {
        if (audioSource != null && entranceRoarClip != null)
        {
            // 한 번 재생되는 사운드를 PlayOneShot으로 재생합니다.
            audioSource.PlayOneShot(entranceRoarClip);
            Debug.Log("보스 등장 사운드 재생 명령 수신 및 실행.");
        }
        else if (audioSource != null && audioSource.clip != null)
        {
            // 만약 entranceRoarClip이 없고, AudioSource 자체에 BGM이 있다면 그것을 재생할 수도 있습니다.
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("BossController: 오디오 소스나 클립이 할당되지 않았습니다.");
        }
    }
    // ⭐ BossSceneLoad.cs에서 시네마틱 종료 후 호출하는 함수 ⭐
    public void StartFirstAttack()
    {
        if (!isDead && playerTransform != null)
        {
            Debug.Log("보스 전투 시작! 첫 공격 개시.");
            StartCoroutine(AttackRoutine());
        }
    }

    // 공격 패턴 코루틴
    IEnumerator AttackRoutine()
    {
        // 보스가 살아있는 동안 무한 반복
        while (!isDead)
        {
            // 플레이어가 존재하는지 확인
            if (playerTransform != null)
            {
                FireAtPlayer();
            }

            // 다음 공격까지 대기
            yield return new WaitForSeconds(attackInterval);
        }
    }

    // 불꽃 발사 로직
    void FireAtPlayer()
    {
        if (firePrefab == null || firePoint == null) return;

        // 1. 플레이어 좌표 계산
        Vector3 targetPosition = playerTransform.position;

        // 2. 불꽃 생성
        GameObject fire = Instantiate(firePrefab, firePoint.position, Quaternion.identity);

        // 3. 불꽃 스크립트에 방향 전달 및 발사 명령
        Fireball fireballScript = fire.GetComponent<Fireball>();
        if (fireballScript != null)
        {
            fireballScript.Launch(targetPosition);
        }
    }

    // 피격 로직 (포탄에 맞았을 때 호출)
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"보스 피격! 남은 체력: {currentHealth}");

        // 🎵 피격 사운드 재생
        if (audioSource != null && hitClip != null)
        {
            audioSource.PlayOneShot(hitClip);
        }
        else
        {
            // 혹시 안 나오면 콘솔에서 이 로그 먼저 확인
            Debug.LogWarning("[BossController] 피격 사운드 재생 불가 (audioSource 또는 hitClip 미할당)");
        }

        // TODO: 피격 애니메이션이나 깜빡임 효과 추가

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        StopAllCoroutines();

        var watcher = FindObjectOfType<PlayerHealthWatcher>();
        if (watcher != null)
            watcher.TriggerClear();
        else
            SceneManager.LoadScene("ending_screen");

        Destroy(gameObject);
    }
    
    
}