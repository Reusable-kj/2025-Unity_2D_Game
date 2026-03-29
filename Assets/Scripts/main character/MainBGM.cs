using UnityEngine;

public class MainBGM : MonoBehaviour
{
    public static MainBGM Instance;

    private AudioSource audioSource;

    private void Awake()
    {
        // 이미 다른 MainBGM이 있으면 자기 자신은 삭제 (중복 방지)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);  // 씬이 바뀌어도 안 없애기

        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.loop = true;    // 혹시 안 켰으면 강제로 루프
            if (!audioSource.isPlaying)
            {
                audioSource.Play();     // 게임 시작 시 자동 재생
            }
        }
    }
}