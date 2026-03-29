using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float moveSpeed = 8f;        // 이동 속도
    public float moveRange = 30f;       // 좌우 이동 범위 (±30)

    private Vector3 startPos;           // 시작 위치
    private int direction = -1;         // 이동 방향 (-1 = 왼쪽, 1 = 오른쪽)
    private float originalScaleX;       // 원래 스케일 (좌우 반전용)

    void Start()
    {
        // 시작 위치 저장
        startPos = transform.position;
        originalScaleX = transform.localScale.x;
    }

    void Update()
    {
        // 이동
        transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);

        // 좌우 범위 체크 후 방향 전환
        if (transform.position.x > startPos.x + moveRange || 
            transform.position.x < startPos.x - moveRange)
        {
            // 캐릭터 좌우 반전
            Vector3 scale = transform.localScale;
            scale.x = originalScaleX * direction;
            transform.localScale = scale;

            // 방향 반전
            direction *= -1;
        }
    }
}