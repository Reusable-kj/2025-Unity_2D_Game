using UnityEngine;

public class GroundMonster : MonoBehaviour
{
    [Header("Move")]
    public float speed = 2f;
    private int dir = 1; // 1 = 오른쪽, -1 = 왼쪽
    private SpriteRenderer sr;

    [Header("Boundaries")]
    public float leftLimit = -40f;
    public float rightLimit = 50f;

    [Header("Randomness")]
    [Range(0f, 1f)] public float randomTurnChancePerFrame = 0.01f;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        ApplyFlip();
    }

    void Update()
    {
        // 랜덤 방향 전환
        if (Random.value < randomTurnChancePerFrame)
        {
            SetDirection(-dir);
        }

        // 이동
        transform.Translate(Vector2.right * dir * speed * Time.deltaTime);

        // 범위 체크
        if (transform.position.x < leftLimit)
        {
            transform.position = new Vector2(leftLimit, transform.position.y);
            SetDirection(1);
        }
        else if (transform.position.x > rightLimit)
        {
            transform.position = new Vector2(rightLimit, transform.position.y);
            SetDirection(-1);
        }
    }

    void SetDirection(int newDir)
    {
        if (dir == newDir) return;
        dir = Mathf.Clamp(newDir, -1, 1);
        ApplyFlip();
    }

    void ApplyFlip()
    {
        if (sr != null) sr.flipX = (dir < 0); 
        // ⚠️ 스프라이트가 원래 오른쪽을 바라보게 그려졌다는 기준
    }
}