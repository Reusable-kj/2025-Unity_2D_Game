using UnityEngine;

public class BookbatMove : MonoBehaviour
{
    public float moveRange = 7f;   // 좌우·상하 범위
    public float speed = 7f;       // 이동 속도

    private Vector2 originPos;     // 시작 위치 기준
    private Vector2 targetPos;     // 랜덤 목표 위치

    private SpriteRenderer sr;     // 이미지 반전용

    void Start()
    {
        originPos = transform.position;
        SetRandomTarget();

        // SpriteRenderer 가져오기
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 목표 지점까지 이동
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // 방향에 따라 이미지 반전
        if (sr != null)
        {
            if (targetPos.x < transform.position.x)
                sr.flipX = true;  // 왼쪽을 볼 때
            else
                sr.flipX = false; // 오른쪽을 볼 때
        }

        // 거의 도착했으면 새로운 목표 지정
        if (Vector2.Distance(transform.position, targetPos) < 0.2f)
        {
            SetRandomTarget();
        }
    }

    void SetRandomTarget()
    {
        float randX = Random.Range(-moveRange, moveRange);
        float randY = Random.Range(-moveRange, moveRange);

        targetPos = new Vector2(originPos.x + randX, originPos.y + randY);
    }
}