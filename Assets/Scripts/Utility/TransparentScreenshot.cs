using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TransparentScreenshot : MonoBehaviour
{
    public Camera targetCamera;     // 스샷 찍을 카메라
    public Transform setPositon;
    public int resolutionWidth = 1920;
    public int resolutionHeight = 1080;

    public GameObject targetObject;
    private GameObject preObject;

    [ContextMenu("Capture Transparent Screenshot")]
    public void Capture()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if(targetObject  == null) return;

        if(preObject != null)
        {
#if UNITY_EDITOR
            DestroyImmediate(preObject);
#else
            Destroy(preObject);
#endif
        }

        preObject = Instantiate(targetObject, setPositon);

        // RenderTexture 생성 (ARGB32는 알파 포함)
        RenderTexture rt = new RenderTexture(resolutionWidth, resolutionHeight, 24, RenderTextureFormat.ARGB32);
        targetCamera.targetTexture = rt;

        // 배경 투명하게 설정
        Color prevColor = targetCamera.backgroundColor;
        CameraClearFlags prevFlag = targetCamera.clearFlags;

        targetCamera.clearFlags = CameraClearFlags.SolidColor;
        targetCamera.backgroundColor = new Color(0, 0, 0, 0);

        // 카메라 렌더링
        targetCamera.Render();

        // 텍스처로 읽기
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(resolutionWidth, resolutionHeight, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, resolutionWidth, resolutionHeight), 0, 0);
        tex.Apply();

        // PNG 저장
        byte[] bytes = tex.EncodeToPNG();
        string path = Path.Combine(Application.dataPath, $"Icon/{targetObject.name}.png");
        File.WriteAllBytes(path, bytes);

        Debug.Log($"투명 배경 스크린샷 저장됨: {path}");

        // 원상 복구
        targetCamera.targetTexture = null;
        RenderTexture.active = null;

#if UNITY_EDITOR
        DestroyImmediate(rt);
#else
        Destroy(rt);
#endif

        targetCamera.clearFlags = prevFlag;
        targetCamera.backgroundColor = prevColor;
    }
}
