using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneStatePair
{ 
    public string sceneName;
    public GameState gameState;
}


public class SceneConfig : SingletonMonoBehaviour<SceneConfig>
{
    [Header("Scene State Settings")]
    public GameState defaultState = GameState.Menu;

    [Header("Scene Configurations")]
    public List<SceneStatePair> sceneConfigs = new List<SceneStatePair>();

    private Dictionary<string, GameState> sceneStates = new Dictionary<string, GameState>();

    protected override void Awake()
    {
        base.Awake();
        GameEvents.OnSceneChanged += OnSceneChanged;
    }

    private void OnDestroy()
    {
        GameEvents.OnSceneChanged -= OnSceneChanged;
    }

    //SceneStatePair의 sceneName과 gameState를 sceneStates에 저장
    private void SetupSceneStates()
    {
        sceneStates.Clear();

        foreach(var config in sceneConfigs)
        {
            if(!string.IsNullOrEmpty(config.sceneName))
            {
                sceneStates[config.sceneName] = config.gameState;
            }
        }
    }

    // 씬 변경 이벤트가 호출 될 때 게임 상태 변경
    private void OnSceneChanged(string sceneName)
    {
        GameState targetState = sceneStates.ContainsKey(sceneName)? sceneStates[sceneName] : defaultState;

        if(GameManager.Instance != null)
        {
            GameManager.Instance.ChangeGameState(targetState);
        }

        Debug.Log($"씬 '{sceneName}' -> 상태:{targetState}");
    }

    // 런타임에서 씬 상태 변경
    public void SetSceneState(string sceneName, GameState targetState)
    {
        sceneStates[sceneName] = targetState;
    }
}
