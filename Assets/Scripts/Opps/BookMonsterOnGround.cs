using UnityEngine;

public class BookMonsterOnGround : MonoBehaviour
{
    public float moveDistance = 5f; // 좌우 이동 범위
    private float speed = 3f;       // 속도 고정

    private float leftLimit;
    private float rightLimit;
    private bool movingRight = true;

    private SpriteRenderer sr; // 이미지 반전용

    void Start()
    {
        float originX = transform.position.x;
        leftLimit = originX - moveDistance;
        rightLimit = originX + moveDistance;

        // SpriteRenderer 연결
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (movingRight)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                new Vector2(rightLimit, transform.position.y),
                speed * Time.deltaTime
            );

            // 오른쪽 이동 시 → 오른쪽 바라보도록
            if (sr != null) sr.flipX = false;

            if (transform.position.x >= rightLimit)
                movingRight = false;
        }
        else
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                new Vector2(leftLimit, transform.position.y),
                speed * Time.deltaTime
            );

            // 왼쪽 이동 시 → 왼쪽 바라보도록
            if (sr != null) sr.flipX = true;

            if (transform.position.x <= leftLimit)
                movingRight = true;
        }
    }
}