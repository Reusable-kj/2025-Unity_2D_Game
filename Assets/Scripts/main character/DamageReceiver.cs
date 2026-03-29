using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    // === 내부 참조 (HP 및 무적 로직) ===
    private PlayerInventory inventory;    
    private PlayerCollision collisionHandler; 

    void Awake()
    {
        // DDOL Player 오브젝트에 붙은 필수 컴포넌트를 찾습니다.
        inventory = GetComponent<PlayerInventory>();
        collisionHandler = GetComponent<PlayerCollision>();

        if (inventory == null || collisionHandler == null)
        {
            Debug.LogError("DamageReceiver: 필수 컴포넌트(Inventory/Collision)가 플레이어에 누락되었습니다. 스크립트 비활성화.");
            enabled = false;
        }
    }

    // 외부 공격체(Fireball, FireTrap)가 호출하는 유일한 함수
    public void TakeDamage(int damageAmount)
    {
        // 1. [무적 확인] PlayerCollision의 canTakeDamage 플래그를 확인합니다.
        // (canTakeDamage 변수는 PlayerCollision에 public으로 선언되어 있어야 합니다.)
        if (collisionHandler.canTakeDamage == false) 
        {
            Debug.Log("DamageReceiver: 무적 시간 중이므로 피해 무시.");
            return; 
        }
        
        // 2. HP 감소 (PlayerInventory에 위임)
        if (inventory.Playerheart > 0)
        {
            inventory.Playerheart -= damageAmount; 
            
            Debug.Log($"DamageReceiver: 피해 적용 완료! 현재 하트: {inventory.Playerheart}");

            // 3. 무적 프레임 시작 명령 (PlayerCollision에 명령)
            // Fireball이나 FireTrap은 물리 충돌이 아니므로 null을 전달합니다.
            collisionHandler.StartDamageInvulnerability(null); 
        }
        
        // 4. 사망 판정은 HealthWatcher의 Update()가 감지하여 처리합니다.
    }
}