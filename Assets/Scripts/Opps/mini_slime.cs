using UnityEngine;

public class SlimeMove : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 4f;         // 이동 속도
    public float moveRange = 20f;         // 이동 범위 (좌우)

    private float startX;                // 시작 위치 X
    private int direction = 1;           // 이동 방향 (1 = 오른쪽, -1 = 왼쪽)

    void Start()
    {
        // 시작 위치 기억
        startX = transform.position.x;
    }

    void Update()
    {
        // 이동
        transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);

        // 범위 체크
        if (transform.position.x > startX + moveRange)
            direction = -1;  // 왼쪽으로 전환
        else if (transform.position.x < startX - moveRange)
            direction = 1;   // 오른쪽으로 전환
    }
}
