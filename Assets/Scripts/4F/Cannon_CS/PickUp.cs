using UnityEngine;

// 맵에 배치된 획득 가능한 포탄 오브젝트에 붙입니다.
public class MortarRoundPickup : MonoBehaviour
{
    private bool playerIsInRange = false; // 플레이어가 습득 범위 안에 있는지
    public KeyCode pickupKey = KeyCode.F; // 습득에 사용할 키 (F 키)

    void Update()
    {
        // 1. 플레이어가 습득 범위 안에 있고, F 키를 눌렀을 때
        if (playerIsInRange && Input.GetKeyDown(pickupKey))
        {
            AttemptPickup();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInRange = true;
            // Debug.Log("습득 가능!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInRange = false;
        }
    }

    void AttemptPickup()
    {
        // 씬에서 CannonController를 찾습니다.
        CanonMove cannonController = FindObjectOfType<CanonMove>(); 
        
        if (cannonController != null)
        {
            cannonController.AcquireMortarRound();
            
            // 습득 완료 후 이 오브젝트는 파괴됩니다.
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("CannonController를 씬에서 찾을 수 없어 포탄 획득 실패.");
        }
    }
}