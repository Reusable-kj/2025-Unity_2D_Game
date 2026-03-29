using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    [Header("Door Visual")]
    public GameObject doorVisual;         // 문 모델(스프라이트) 오브젝트

    public string playerTag = "Player";   // 플레이어 태그

    private bool isPlayerNearby = false;
    private PlayerInventory playerInventory; // 인벤토리.cs 속 인자 참조

    public upstair upStairScript; // 계단 스크립트 받아오기

    [Header("Door Sounds")]
    [SerializeField] private AudioSource doorAudioSource; // 소리 재생용
    [SerializeField] private AudioClip openClip;          // 문 열릴 때 소리
    [SerializeField] private AudioClip failClip;          // 열쇠 없을 때 소리(있으면)

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.Alpha1))
        {
            TryOpenDoor();
        }
    }

    void TryOpenDoor()
    {
        if (playerInventory == null)
        {
            Debug.LogError("playerInventory가 null입니다!");
            return;
        }

        if (upStairScript == null)
        {
            Debug.LogError("upStairScript가 null입니다!");
            return;
        }

        // 🔹 키가 있을 때만 문 열기
        if (playerInventory.UseKeyIfHas())
        {
            upStairScript.isLocked = false;

            Debug.Log("문 열림!");
            Debug.Log(upStairScript.isLocked);

            // ✅ 문 열리는 소리 재생
            PlayDoorSound(openClip);

            // ✅ '문 그림'만 끄기 (소리 나는 오브젝트는 그대로 두기!)
            if (doorVisual != null)
                doorVisual.SetActive(false);
            // 만약 doorVisual을 따로 안 쓰고 이 오브젝트 자체를 문으로 쓴다면
            // gameObject.SetActive(false); 대신 스프라이트/콜라이더만 끄는 걸 추천
        }
        else
        {
            Debug.Log("열쇠가 없어서 문을 열 수 없음");
        }
    }

    void PlayDoorSound(AudioClip clip)
    {
        if (doorAudioSource == null || clip == null) return;

        doorAudioSource.clip = clip;
        doorAudioSource.Play();
        Debug.Log("[DoorInteraction] 문 소리 재생: " + clip.name);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerNearby = true;
            playerInventory = other.GetComponent<PlayerInventory>();
            upStairScript = other.GetComponent<upstair>();
            Debug.Log("플레이어 접근");
            Debug.Log("playerInventory: " + playerInventory);
            Debug.Log("upStairScript: " + upStairScript);
            if (playerInventory != null)
                Debug.Log("haskey 값: " + playerInventory.hasKey);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerNearby = false;
            playerInventory = null;
            upStairScript = null;
        }
    }
}