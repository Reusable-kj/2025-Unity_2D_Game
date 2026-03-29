using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthWatcher : MonoBehaviour
{
    [Header("▶ 게임 플레이 UI 루트들")]
    [SerializeField] private GameObject inventoryUIRoot;  // 인벤토리(슬롯들) - 지금 쓰는 그거
    [SerializeField] private GameObject healthUIRoot_1;     // 하트 3개 묶어놓은 부모
    [SerializeField] private GameObject healthUIRoot_2;     // 하트 2개 묶어놓은 부모
    [SerializeField] private GameObject healthUIRoot_3;
    [SerializeField] private GameObject keyUIRoot;       // Key / Gray_posion / Heal_posion 묶어놓은 부모
    [SerializeField] private GameObject Gray_posion;
    [SerializeField] private GameObject Heal_posion;
    [SerializeField] private GameObject boomUIRoot;       // boom 아이콘 오브젝트

    [Header("▶ 씬 이름")]
    [SerializeField] private string restartSceneName = "restart_screen";
    [SerializeField] private string clearSceneName = "ending_screen";

    private bool isDead = false;
    private bool isClear = false;

    // Restart 버튼에서 호출
    public void ResetDeadFlag()
    {
        isDead = false;
        isClear = false;
    }

    // Restart 버튼에서 호출 (이름은 그대로 두고, 이제 전체 UI를 다시 켜게 만들기)
    public void ShowInventoryUI()
    {
        SetGameplayUIVisible(true);

        // 재시작할 땐 포탄/클리어 상태 초기화
        if (BoomUI.Instance != null)
        {
            BoomUI.Instance.SetBoom(false);   // 처음엔 BOOM 아이콘 항상 꺼두기
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (isDead || isClear) return;        // 이미 끝난 상태면 체크 중지
        if (PlayerInventory.Instance == null) return;

        int hp = PlayerInventory.Instance.Playerheart;

        if (hp <= 0)
        {
            HandlePlayerDead();
        }
    }

    /* ---------------- 공통 UI 토글 함수 ---------------- */

    private void SetGameplayUIVisible(bool visible)
    {
        if (inventoryUIRoot != null)
            inventoryUIRoot.SetActive(visible);

        if (healthUIRoot_1 != null)
            healthUIRoot_1.SetActive(visible);

        if (healthUIRoot_2 != null)
            healthUIRoot_2.SetActive(visible);

        if (healthUIRoot_3 != null)
            healthUIRoot_3.SetActive(visible);

        if (keyUIRoot != null)
            keyUIRoot.SetActive(visible);

        if (Gray_posion != null)
            Gray_posion.SetActive(visible);

        if (Heal_posion != null)
            Heal_posion.SetActive(visible);

        if (boomUIRoot != null)
            boomUIRoot.SetActive(visible);
    }

    /* ---------------- 죽었을 때 처리 ---------------- */

    private void HandlePlayerDead()
    {
        if (isDead) return;
        isDead = true;

        // 게임 플레이 UI 전부 끄기
        SetGameplayUIVisible(false);

        Time.timeScale = 1f;
        SceneManager.LoadScene(restartSceneName);
    }

    /* ---------------- 클리어(보스 죽음 등) 처리 ---------------- */

    public void TriggerClear()
    {
        if (isClear || isDead) return;
        isClear = true;

        // 게임 플레이 UI 전부 끄기
        SetGameplayUIVisible(false);

        Time.timeScale = 1f;
        SceneManager.LoadScene(clearSceneName);
    }
}
