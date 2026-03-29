using UnityEngine;

public class IronBarMove : MonoBehaviour
{
    public float moveDistance = 15f;   // 앞뒤로 이동할 거리
    public float speed = 3f;           // 이동 속도

    private float startX;
    private int direction = 1;
    private SpriteRenderer sr;

    void Start()
    {
        startX = transform.position.x;
        sr = GetComponent<SpriteRenderer>();   // 스프라이트 가져오기
    }

    void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        if (transform.position.x > startX + moveDistance)
            ChangeDirection(-1);
        else if (transform.position.x < startX - moveDistance)
            ChangeDirection(1);
    }

    void ChangeDirection(int newDir)
    {
        direction = newDir;

        // 🎨 방향에 따라 스프라이트 뒤집기
        if (sr != null)
            sr.flipX = (direction == -1);
    }
}