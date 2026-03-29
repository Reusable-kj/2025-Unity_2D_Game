using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.3f;

    public static CameraFollow Instance;
    private float targetY;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.Find("main_character_9");
            if (player != null) target = player.transform;
        }

        if (target != null)
        {
            float minY = GetMinY();
            transform.position = new Vector3(
                target.position.x,
                Mathf.Max(target.position.y, minY),
                transform.position.z
            );
        }
    }

    void LateUpdate()
    {
        // 📌 줌 중이라면 카메라 이동을 멈춤
        if (AimingCameraZoom.IsZoomed) return;

        if (target == null) return;

        string scene = SceneManager.GetActiveScene().name;

        if (scene.Contains("Floor_2"))
        {
            Vector3 lockYPosition = new Vector3(
                target.position.x,
                0f,
                transform.position.z
            );

            transform.position = Vector3.Lerp(
                transform.position,
                lockYPosition,
                smoothSpeed * Time.deltaTime * 60f
            );
            return;
        }

        float minY = GetMinY();
        targetY = Mathf.Max(target.position.y, minY);

        Vector3 desiredPosition = new Vector3(
            target.position.x,
            targetY,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime * 60f
        );
    }

    private float GetMinY()
    {
        string scene = SceneManager.GetActiveScene().name;

        if (scene.Contains("Floor_4"))
            return 8.7f;

        return 0f;
    }
}
