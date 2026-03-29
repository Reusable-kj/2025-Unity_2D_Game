using UnityEngine;

public class DisplayGuidePanel : MonoBehaviour
{
    public GameObject guidePanel;     // 가이드 패널
    public GameObject gameStartPanel;     // 시작 버튼 있는 패널(숨길거면)

    // 가이드 버튼 클릭 시 실행될 함수
    public void OpenGuide()
    {
        Debug.Log("OpenGuide 눌림");
        guidePanel.SetActive(true);       // 가이드 패널 켜기
        if (gameStartPanel != null)
            gameStartPanel.SetActive(false);  // 시작 패널 숨기기 (원하면)
    }

    // 닫기 버튼 클릭 시 실행될 함수
    public void CloseGuide()
    {
        guidePanel.SetActive(false);      // 가이드 패널 끄기
        if (gameStartPanel != null)
            gameStartPanel.SetActive(true);   // 시작 패널 다시 보이기
    }
}
