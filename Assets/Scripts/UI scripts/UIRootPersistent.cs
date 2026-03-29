using UnityEngine;

public class UIRootPersistent : MonoBehaviour
{
    private static UIRootPersistent _instance;

    private void Awake()
    {
        // 이미 다른 UIRoot가 있다면 자기 자신은 삭제
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
