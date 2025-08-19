using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이벤트 연출용 클래스
public class EventSystem : SingletonMonoBehaviour<EventSystem>
{
    public bool isEventPlaying = false;

    //이벤트 시작
    public void StartEvent()
    {
        if (isEventPlaying) return;                 //이미 이벤트 진행중이면 return

        isEventPlaying = true;
        GameManager.Instance?.PauseGame();          //이벤트가 시작되면 게임 상태 변경

        Debug.Log("이벤트 시작 - 게임 일시정지");
    }

    //이벤트 종료
    public void EndEvent()
    {
        if(!isEventPlaying) return;                 //이벤트가 진행중이 아니라면 return

        isEventPlaying = false;
        GameManager.Instance?.ResumeGame();         //이벤트가 종료되면 게임 재개

        Debug.Log("이벤트 종료 - 게임 재개");
    }

    //이벤트 연출 예제
    public void PlayCutScene(float duration)
    {
        StartCoroutine(CutSceneCoroutine(duration));
    }

    private IEnumerator CutSceneCoroutine(float duration)
    {
        StartEvent();   //이벤트 시작

        Debug.Log($"{duration}초 동안 컷신 재생");
        yield return new WaitForSecondsRealtime(duration);

        EndEvent();     //이벤트 종료
    }
}
