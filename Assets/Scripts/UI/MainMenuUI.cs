using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Button startGameButton;
    public Button startGameWithLoadingButton;

    // Start is called before the first frame update
    void Start()
    {
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(() => {
                SceneManager.Instance.LoadScene("GameScene");
            });
        }

        if (startGameWithLoadingButton != null)
        {
            startGameWithLoadingButton.onClick.AddListener(() => {
                SceneManager.Instance.LoadSceneWithLoadingScreen("GameScene");
            });
        }
    }

    
}
