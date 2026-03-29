using UnityEngine;
using System.Collections;
using System.Linq; 
// 이 스크립트는 제미나이가 짜준 코드로, 전체적으로 씬이 넘어갔을 때 애니메이션 효과를 위한 스크립트입니다.

// 씬 로드 시 보스를 비춘 뒤, 맵 전경을 보여주고 다시 플레이어에게 카메라가 이동하고, 오브젝트를 임시 파괴합니다.(카메라 오류 방지)

// 카메라가 이동하고 있을때는 플레이어의 동작이 제한됩니다.
public class BossSceneLoad : MonoBehaviour
{
    // === 인스펙터 연결 변수 ===
    [Header("4층 맵 시작 위치 (플레이어 이동 좌표)")]
    public Vector3 bossSceneStartPos = new Vector3(0f, 40f, 0f);

    [Header("보스 시네마틱 타겟")]
    public Transform bossTarget;
    
    // === 카메라 연출 설정 ===
    public float bossFocusDuration = 2.0f;
    public float panoramaZoomSize = 25.0f;
    public float normalZoomSize = 10.0f;
    public float moveDuration = 1.0f;

    // === 내부 관리 변수 ===
    private Camera mainCamera;
    private Movement playerMovement; 
    private bool isCinematicPlaying = false; 

    // ⭐ 1. CameraFollow 스크립트를 제어하기 위한 변수 추가 ⭐
    // (CameraFollow 라는 이름의 클래스가 있다고 가정합니다)
    private MonoBehaviour cameraFollowScript; 
    
    private BossController bossController;
    private AudioSource audioSource;
    void Start()
    {
        Debug.Log("BossSceneLoad Start() 실행됨."); 
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
        
        // 플레이어 찾기
        PlayerInventory playerInventory = FindObjectOfType<PlayerInventory>();
        if (playerInventory != null)
        {
            playerMovement = playerInventory.GetComponent<Movement>();
        }

        // ⭐ 2. 필수 오브젝트 누락 검사 ⭐
        // ⭐ 2. 씬에 있는 CameraFollow 스크립트를 찾아옵니다. ⭐
        // 만약 스크립트 이름이 CameraFollow가 아니라면, 그 이름으로 바꿔주세요.
        cameraFollowScript = mainCamera.GetComponent<CameraFollow>();

        bossController = FindObjectOfType<BossController>();
        
        if (mainCamera == null || playerMovement == null || bossTarget == null)
        {
            Debug.LogError("BossSceneLoad: 필수 오브젝트 누락. 시네마틱 취소.");
            Destroy(gameObject);
            return; 
        }

        if (!isCinematicPlaying)
        {
            StartCoroutine(MoveToBossScene());
        }
    }

    IEnumerator MoveToBossScene()
    {
        isCinematicPlaying = true;
        Transform playerTransform = playerMovement.transform;
        
        // =========================================================
        // ⭐ 3. 시네마틱 시작 전, CameraFollow 스크립트 비활성화 ⭐
        // =========================================================
        if (cameraFollowScript != null)
        {
            cameraFollowScript.enabled = false; // 이제 카메라가 플레이어를 따라다니지 않습니다.
            Debug.Log("CameraFollow 스크립트 비활성화됨.");
        }

        // 0. 즉시 텔레포트
        Vector3 playerStartPos = new Vector3(bossSceneStartPos.x, bossSceneStartPos.y, playerTransform.position.z);
        playerTransform.position = playerStartPos;
        
        Vector3 cameraStartPos = new Vector3(playerStartPos.x, playerStartPos.y, mainCamera.transform.position.z);
        mainCamera.transform.position = cameraStartPos;
        
        // 1. 준비 (플레이어 조작 잠금)
        playerMovement.LockMovement();
        yield return new WaitForSeconds(0.5f); 

        // 2. 카메라 보스 포커스
        Vector3 bossFocusTarget = new Vector3(bossTarget.position.x, bossTarget.position.y, mainCamera.transform.position.z);
        yield return StartCoroutine(SmoothCameraMove(mainCamera.transform.position, bossFocusTarget, mainCamera.orthographicSize, normalZoomSize, moveDuration));
        if (bossController != null)
        {
            bossController.BossRoarSound();
        }
        yield return new WaitForSeconds(bossFocusDuration);
        
        // 3. 전체 맵 파노라마 (줌 아웃)
        Vector3 panoramaCenter = Vector3.Lerp(playerTransform.position, bossTarget.position, 0.7f);
        panoramaCenter.z = mainCamera.transform.position.z; 
        
        yield return StartCoroutine(SmoothCameraMove(mainCamera.transform.position, panoramaCenter, mainCamera.orthographicSize, panoramaZoomSize, moveDuration * 1.5f));
        
        yield return new WaitForSeconds(1.5f); 

        // 4. 플레이어에게 복귀 및 줌 인
        Vector3 playerFollowTarget = new Vector3(playerTransform.position.x + 10f, playerTransform.position.y + 6f, mainCamera.transform.position.z);
        
        yield return StartCoroutine(SmoothCameraMove(mainCamera.transform.position, playerFollowTarget, mainCamera.orthographicSize, normalZoomSize, moveDuration));

        // =========================================================
        // ⭐ 4. 시네마틱 종료 후, CameraFollow 스크립트 다시 활성화 ⭐
        // =========================================================
        if (cameraFollowScript != null)
        {
            // 카메라가 플레이어 위치로 돌아왔으니, 다시 추적을 시작합니다.
            cameraFollowScript.enabled = true; 
            Debug.Log("CameraFollow 스크립트 활성화됨.");
        }

        // 5. 시네마틱 종료 및 게임 시작
        playerMovement.UnlockMovement();
        
        //bossController.StartFirstAttack(); 
        bossController.StartFirstAttack(); 
        
        Debug.Log("보스 시네마틱 종료. 전투 시작!");
        
        Destroy(gameObject); 
    }
    
    IEnumerator SmoothCameraMove(Vector3 startPos, Vector3 endPos, float startSize, float endSize, float duration)
    {
        float time = 0;
        
        while (time < duration)
        {
            float t = time / duration;
            // SmoothStep을 쓰면 시작과 끝이 더 부드러워집니다 (선택사항)
            // t = t * t * (3f - 2f * t); 
            
            mainCamera.transform.position = Vector3.Lerp(startPos, endPos, t);
            mainCamera.orthographicSize = Mathf.Lerp(startSize, endSize, t);
            
            time += Time.deltaTime;
            yield return null; 
        }
        
        mainCamera.transform.position = endPos;
        mainCamera.orthographicSize = endSize;
    }
    
}