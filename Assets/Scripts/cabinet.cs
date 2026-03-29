using UnityEngine;

public class KeyCabinet : MonoBehaviour
{
    [Header("숨겨진 키 오브젝트")]
    public GameObject keyObject;

    [Header("F키 누르고 있어야 하는 시간")]
    public float holdTime = 1f;

    [Header("박스 애니메이터")]
    public Animator boxAnimator;

    [Header("애니메이터 파라미터 이름")]
    public string openBoolName = "open";       // 열린 상태 유지
    public string holdBoolName = "isHolding";  // F키 누르는 동안
    public string rangeBoolName = "inRange";   // 플레이어 근접 표시

    [Header("언제 박스를 열까?")]
    public bool openOnReveal = true;   // 키 드랍 시 열기
    public bool openOnPickup = false;  // 키 획득 시 열기

    [Header("사운드")]
    [SerializeField] private AudioSource audioSource;   // 열쇠 줍는 효과음 재생기
    [SerializeField] private AudioClip keyPickupClip;   // 열쇠 줍는 소리 클립

    private bool keyRevealed = false;
    private float holdTimer = 0f;
    private bool isPlayerInRange = false;
    private GameObject player;
    private bool boxOpened = false;

    private void Awake()
    {
        // 인스펙터에서 안 넣어줬으면 같은 오브젝트의 AudioSource 자동으로 가져오기
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogWarning("[KeyCabinet] AudioSource가 연결되지 않았고, 오브젝트에도 없습니다.");
            }
        }
    }

    void Update()
    {
        if (!isPlayerInRange) return;

        // ▼ F키 누르는 동안 '누르는 애니메이션' 재생
        if (!keyRevealed)
        {
            if (Input.GetKeyDown(KeyCode.F))
                SetHold(true);

            if (Input.GetKey(KeyCode.F))
            {
                holdTimer += Time.deltaTime;
                if (holdTimer >= holdTime)
                    RevealKey(); // 드랍/등장 + (옵션) 열기
            }

            if (Input.GetKeyUp(KeyCode.F))
            {
                holdTimer = 0f;
                SetHold(false); // 손 떼면 'Hold' 해제
            }
        }
        else
        {
            // ▼ 키가 이미 나왔으면 F 한번 눌러 획득
            if (Input.GetKeyDown(KeyCode.F))
                TryPickupKey();
        }
    }

    void RevealKey()
    {
        if (keyRevealed) return; // 중복 방지

        // 1) 키 오브젝트 등장
        if (keyObject != null)
        {
            keyObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("[KeyCabinet] keyObject가 할당되지 않았습니다.");
        }

        keyRevealed = true;
        Debug.Log("열쇠가 나타났습니다!");

        // 2) F키 누르는 애니메이션 종료(선택)
        SetHold(false);

        // 3) 드랍 시 박스 열기 옵션 반영
        if (openOnReveal)
            OpenBox();
    }

    void TryPickupKey()
    {
        PlayerInventory inventory = player ? player.GetComponent<PlayerInventory>() : null;
        if (inventory != null)
        {
            inventory.GiveKey();
            Debug.Log("열쇠를 획득했습니다!");

            // 🔊 열쇠 줍는 소리 재생
            if (audioSource != null)
            {
                if (keyPickupClip != null)
                {
                    audioSource.PlayOneShot(keyPickupClip);
                }
                else if (audioSource.clip != null)
                {
                    audioSource.Play(); // AudioSource에 직접 클립이 들어있다면 이걸로 재생
                }
                else
                {
                    Debug.LogWarning("[KeyCabinet] 재생할 AudioClip이 없습니다.");
                }
            }
            else
            {
                Debug.LogWarning("[KeyCabinet] AudioSource가 없습니다.");
            }

            if (keyObject != null)
                keyObject.SetActive(false);

            if (openOnPickup)
                OpenBox();

            enabled = false; // 1회성
        }
    }

    void OpenBox()
    {
        if (boxOpened) return;
        boxOpened = true;

        if (boxAnimator)
        {
            // 열린 상태 고정
            boxAnimator.SetBool(openBoolName, true);
            // 혹시 Hold가 남아있다면 끄기
            boxAnimator.SetBool(holdBoolName, false);
        }
    }

    void SetHold(bool v)
    {
        if (boxAnimator)
            boxAnimator.SetBool(holdBoolName, v);
    }

    void SetInRange(bool v)
    {
        if (boxAnimator)
            boxAnimator.SetBool(rangeBoolName, v);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            player = other.gameObject;
            holdTimer = 0f;
            SetInRange(true); // 근접 애니메이션 on
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            player = null;
            holdTimer = 0f;
            SetHold(false);   // 범위 벗어나면 강제 해제
            SetInRange(false);
        }
    }

    void OnDisable()
    {
        // 씬 전환/비활성화 시 깔끔히 리셋(선택)
        if (boxAnimator)
        {
            boxAnimator.SetBool(holdBoolName, false);
            boxAnimator.SetBool(rangeBoolName, false);
        }
    }
}