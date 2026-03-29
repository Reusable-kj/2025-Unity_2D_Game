using UnityEngine;
using UnityEngine.SceneManagement;

public class AimingCameraZoom : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.X;
    public string targetScene = "Floor_4_jump_map";

    public Vector2 zoomCameraOffset = new Vector2(55f, 30f);
    public float zoomSize = 30f;

    private Camera cam;
    private CanonMove canon;   // ⭐ 추가: CanonMove 참조
    private float defaultSize;
    private Vector3 defaultPos;

    public static bool IsZoomed = false;

    void Start()
    {
        cam = Camera.main;
        canon = FindObjectOfType<CanonMove>();   // ⭐ 추가: CanonMove 찾기
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != targetScene) return;
        if (canon == null) return;

        // ⭐ 추가: CanonMove에서 Aiming 상태일 때만 X키 작동
        if (Input.GetKeyDown(toggleKey))
            ToggleZoom();
    }

    void ToggleZoom()
    {
        IsZoomed = !IsZoomed;

        if (IsZoomed)
        {
            defaultSize = cam.orthographicSize;
            defaultPos = cam.transform.position;

            cam.orthographicSize = zoomSize;
            cam.transform.position = new Vector3(
                zoomCameraOffset.x,
                zoomCameraOffset.y,
                defaultPos.z
            );
        }
        else
        {
            cam.orthographicSize = defaultSize;

            Vector3 restored = defaultPos;

            if (SceneManager.GetActiveScene().name.Contains("Floor_4"))
            {
                restored.y -= 8.7f;
            }

            cam.transform.position = restored;
            
        }
    }
}