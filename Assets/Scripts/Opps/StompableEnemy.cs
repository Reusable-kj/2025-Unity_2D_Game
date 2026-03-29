using UnityEngine;

public class StompableEnemy : MonoBehaviour
{
    // 필요시 체력/사운드/애니 등을 확장하세요.
    [Tooltip("충돌을 무시할 대상(철곤 괴물의 Collider2D)")]
    public Collider2D enemyCollider;   // 인스펙터에 드래그

    void Awake()
    {
        var myCol = GetComponent<Collider2D>();
        if (myCol && enemyCollider)
        {
            Physics2D.IgnoreCollision(myCol, enemyCollider, true);
        }
    }
}