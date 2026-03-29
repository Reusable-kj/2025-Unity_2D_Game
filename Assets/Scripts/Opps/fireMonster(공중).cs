using UnityEngine;

public class FlyingMonster : MonoBehaviour
{
    [Header("Move")]
    public float speed = 2f;
    public float moveChangeTime = 2f;  
    private Vector2 targetPos;
    private float timer;
    private SpriteRenderer sr;

    [Header("Boundaries")]
    public float leftLimit = -5f;    // 왼쪽 맵 경계 보정
    public float rightLimit = 60f;   // 오른쪽 넉넉하게
    public float bottomLimit = 7f;   // 세로 아래
    public float topLimit = 15f;     // 세로 위

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        PickNewTarget();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            PickNewTarget();
        }

        // 이동
        Vector2 oldPos = transform.position;
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // 위치 보정 (절대 맵 밖으로 못 나가게)
        float clampedX = Mathf.Clamp(transform.position.x, leftLimit, rightLimit);
        float clampedY = Mathf.Clamp(transform.position.y, bottomLimit, topLimit);
        transform.position = new Vector2(clampedX, clampedY);

        // 방향에 맞춰 flip
        Vector2 moveDir = (Vector2)transform.position - oldPos;
        if (moveDir.x != 0) sr.flipX = (moveDir.x < 0);

        // 목표 도달 → 새 타겟
        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            PickNewTarget();
        }
    }

    void PickNewTarget()
    {
        float x = Random.Range(leftLimit, rightLimit);
        float y = Random.Range(bottomLimit, topLimit);
        targetPos = new Vector2(x, y);
        timer = moveChangeTime;
    }
}