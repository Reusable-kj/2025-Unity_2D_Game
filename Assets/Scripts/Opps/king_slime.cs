using UnityEngine;

public class KingSlimeMove : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 4f;       // 이동 속도
    public float moveRange = 20f;      // 이동 범위 (좌우)
    public float changeDirDelay = 2.5f;  // 방향 랜덤 변경 간격 (초)

    private float startX;              // 시작 위치 X
    private int direction = 1;         // 이동 방향 (1 = 오른쪽, -1 = 왼쪽)
    private float nextChangeTime;      // 방향 랜덤 변경 타이머

    private SpriteRenderer sr;         // 이미지 반전용

    void Start()
    {
        // 시작 위치 기억
        startX = transform.position.x;

        // SpriteRenderer 가져오기
        sr = GetComponent<SpriteRenderer>();

        // 첫 방향 전환 타이머 세팅
        nextChangeTime = Time.time + changeDirDelay;
    }

    void Update()
    {
        // 이동
        transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);

        // 범위 제한 (넘어가면 무조건 반대 방향)
        if (transform.position.x > startX + moveRange)
        {
            direction = -1;
            Flip();
        }
        else if (transform.position.x < startX - moveRange)
        {
            direction = 1;
            Flip();
        }

        // 일정 시간마다 랜덤 방향 전환
        if (Time.time >= nextChangeTime)
        {
            // 50% 확률로 방향 반전
            if (Random.value > 0.5f)
            {
                direction *= -1;
                Flip();
            }

            // 다음 체크 시간 갱신
            nextChangeTime = Time.time + changeDirDelay;
        }
    }

    // 이미지 반전 (X축)
    void Flip()
    {
        if (sr != null)
            sr.flipX = direction < 0;   // 왼쪽 바라볼 때 flipX 켜기
    }
}