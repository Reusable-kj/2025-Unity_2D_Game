using UnityEngine;
using UnityEngine.SceneManagement;

public class upstair : MonoBehaviour
{
    private Collider2D currentCollider = null;

    [Header("다음 층 스폰 위치")]
    public Vector3 targetSpawnPosition = new Vector3(1f, 0f, 0f);

    [Header("계단 잠금")]
    public bool isLocked = true;

    [Header("현재 층 번호 (1~5)")]
    public int currentFloor = 1;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isLocked) return;               // 잠겨있으면  종료
        if (currentCollider == null) return; // 계단 트리거 안 아니면 종료

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("[upstair] F 입력 → 위층 이동");
            MoveUpstairs();
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Respawn"))
        {
            currentCollider = other;
            Debug.Log("[upstair] 계단 범위 진입");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other == currentCollider)
        {
            currentCollider = null;
            Debug.Log("[upstair] 계단 범위 이탈");
        }
    }

    void MoveUpstairs()
    {
        //  씬 로드 전에 플레이어 위치를 미리 이동
        transform.position = targetSpawnPosition;

        // 물리값 초기화(충돌 방지)
        if (rb)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        int nextFloor = currentFloor + 1;
        currentFloor = nextFloor;
        isLocked = true;
        
        switch (nextFloor)
        {
            case 2:
                SceneManager.LoadScene("Floor_2");
                isLocked = false;
                break;
            case 3:
                SceneManager.LoadScene("jump_map");
                break;
            case 4:
                SceneManager.LoadScene("Floor_3");
                break;
            case 5:
                SceneManager.LoadScene("Floor_4_jump_map");
                Debug.Log($"[DEBUG] Player Y = {transform.position.y}");
                break;
            default:
                Debug.Log("[upstair] 더 이상 올라갈 층이 없음");
                return;
        }

        Debug.Log($"[upstair] Floor{nextFloor} 로 이동");
    }
}
