using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour
{
    private void Start()
    {
        // UI_Scene이 안 떠 있으면 Additive로 로드
        var uiScene = SceneManager.GetSceneByName("UI_Scene");
        if (!uiScene.isLoaded)
        {
            SceneManager.LoadScene("UI_Scene", LoadSceneMode.Additive);
        }
    }
}
