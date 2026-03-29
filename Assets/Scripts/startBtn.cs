using UnityEngine;
using UnityEngine.SceneManagement;

public class StrBtn : MonoBehaviour
{
    // 시작할 씬 이름 (빌드 세팅에 등록되어 있어야 함)
    [SerializeField] private string targetSceneName = "Floor_1";

    public void OnClickStart()
    {
        // 혹시 멈춰있을까봐
        Time.timeScale = 1f;

        // 씬 이동
        SceneManager.LoadScene(targetSceneName);
    }
}