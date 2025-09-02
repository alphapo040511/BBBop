using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneQuickLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.Instance.LoadScene(sceneName);
    }
}
