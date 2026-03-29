using UnityEngine;
using UnityEngine.UI;

public class BoomUI : MonoBehaviour
{
    // 싱글톤 (어디서나 BoomUI.Instance 로 접근)
    public static BoomUI Instance;

    [Header("BOOM 아이콘 이미지 (이름: boom)")]
    [SerializeField] private Image boomImage;   // boom 이미지 컴포넌트

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Inspector에서 안 넣어줬으면, 이름으로 자동 검색
        if (boomImage == null)
        {
            GameObject boomObj = GameObject.Find("boom"); // 계층창 이름이 "boom" 인 오브젝트
            if (boomObj != null)
            {
                boomImage = boomObj.GetComponent<Image>();
            }
            else
            {
                Debug.LogWarning("BoomUI: 씬에서 이름이 'boom' 인 오브젝트를 찾지 못했습니다.");
            }
        }
    }

    private void Start()
    {
        // 시작할 때는 항상 숨겨둔다
        SetBoom(false);
    }

    /// <summary>
    /// hasBoom == true 면 아이콘 켜기, false 면 숨기기
    /// </summary>
    public void SetBoom(bool hasBoom)
    {
        if (boomImage != null)
        {
            // Image 컴포넌트의 활성/비활성 대신, 오브젝트 자체를 켜고 끕니다.
            boomImage.gameObject.SetActive(hasBoom);
        }
        else
        {
            Debug.LogWarning("BoomUI: boomImage 가 설정되어 있지 않습니다.");
        }
    }
}
