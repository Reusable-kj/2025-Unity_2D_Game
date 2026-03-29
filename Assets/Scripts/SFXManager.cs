using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Clips")]
    public AudioClip jumpClip;
    public AudioClip keyPickupClip;
    public AudioClip doorOpenClip;
    public AudioClip hitClip;           // 공격당할 때
    public AudioClip cannonFireClip;    // 대포 발사
    public AudioClip cannonHitClip;     // 대포 적중

    private void Awake()
    {
        // 싱글톤 + 중복 생성 방지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);  // 씬 넘어가도 유지

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    // 내부 공용 재생 함수
    private void PlayOneShot(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip);
    }

    // 외부에서 쓸 함수들
    public void PlayJump()        => PlayOneShot(jumpClip);
    public void PlayKeyPickup()   => PlayOneShot(keyPickupClip);
    public void PlayDoorOpen()    => PlayOneShot(doorOpenClip);
    public void PlayHit()         => PlayOneShot(hitClip);
    public void PlayCannonFire()  => PlayOneShot(cannonFireClip);
    public void PlayCannonHit()   => PlayOneShot(cannonHitClip);
}