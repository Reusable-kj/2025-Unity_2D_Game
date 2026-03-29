using UnityEngine;

public class MonoObject: MonoBehaviour
{
    public static MonoObject Instance;
    private void Awake()
    {
        // ★ 싱글톤 + DontDestroyOnLoad
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
