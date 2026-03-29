using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class UICameraBinder : MonoBehaviour
{
    private void Start()
    {
        var canvas = GetComponent<Canvas>();

        if (canvas.renderMode != RenderMode.ScreenSpaceCamera)
            return;

        if (canvas.worldCamera == null)
        {
            // Tag가 MainCamera인 카메라를 찾아서 연결
            canvas.worldCamera = Camera.main;
        }
    }
}
