using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    [SerializeField] private string restartSceneName = "start_screen"; // 처음 시작 씬 이름
    [SerializeField] private int restartHeart = 3;                // 다시 시작할 때 체력
     
    public void OnClickRestart()
    {
        Time.timeScale = 1f;
        var stair = FindObjectOfType<upstair>();
        if (stair != null)
        {
            stair.currentFloor = 1;
            stair.isLocked = false; // 필요하면 같이!
        }

        ResetCameraState();
        
        // 1) 체력 다시 채우기
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.Playerheart = restartHeart;
        }

        // 2) 다시 죽을 수 있게 플래그/체력바 상태 리셋
        var watcher = FindObjectOfType<PlayerHealthWatcher>();
        if (watcher != null)
        {
            watcher.ResetDeadFlag();   // isDead = false
            watcher.ShowInventoryUI(); // 체력바/인벤토리 다시 보이게
        }
        
        Movement.IsRestarting = true;
        Movement.RestartSpawnPoint = new Vector3(-21f, -3f, 0f);

        // 3) 게임 첫 씬으로 이동
        SceneManager.LoadScene(restartSceneName);
    }

    private void ResetCameraState()
    {
        // 🌟 Zoom 상태 초기화
        AimingCameraZoom.IsZoomed = false;

        Camera cam = Camera.main;
        if (cam == null) return;

        // Floor_1 기본 카메라 값으로 복귀
        cam.orthographicSize = 5f; // 네 기본 FOV로 수정
        cam.transform.position = new Vector3(
            0f, // Floor_1에서는 Offset 없음
            0f,
            cam.transform.position.z
        );
    }
}