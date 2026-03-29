using UnityEngine;

public class FIREMonsterMove : MonoBehaviour
{
    [Header("이동 범위 (Min ~ Max)")]
    public Vector2 minPos = new Vector2(-25f, -2f);
    public Vector2 maxPos = new Vector2(60f, 4f);

    [Header("이동 설정")]
    public float speed = 2f;
    public float minMoveTime = 1f;
    public float maxMoveTime = 3f;

    private SpriteRenderer sr;
    private Vector2 targetPos;
    private float timer;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        SetRandomMove();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        // 🔹 목표 위치까지 이동
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // ⏱ 목표 도착 or 시간 종료 → 다음 랜덤 목적지 선정
        if (Vector2.Distance(transform.position, targetPos) < 0.2f || timer <= 0f)
            SetRandomMove();

        // 🔄 방향 따라 이미지 반전 (왼쪽 이동 시 flipX)
        if (sr != null)
            sr.flipX = (targetPos.x < transform.position.x);

        // 🛑 범위 밖으로 절대 못 나가게 제한
        float clampX = Mathf.Clamp(transform.position.x, minPos.x, maxPos.x);
        float clampY = Mathf.Clamp(transform.position.y, minPos.y, maxPos.y);
        transform.position = new Vector2(clampX, clampY);
    }

    void SetRandomMove()
    {
        timer = Random.Range(minMoveTime, maxMoveTime);

        // 🎯 X,Y 모두 범위 내 랜덤
        float randomX = Random.Range(minPos.x, maxPos.x);
        float randomY = Random.Range(minPos.y, maxPos.y);

        targetPos = new Vector2(randomX, randomY);
    }
}