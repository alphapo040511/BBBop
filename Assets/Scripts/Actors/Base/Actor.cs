using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 액터 베이스 클래스
public class Actor : MonoBehaviour
{
    public bool canUpdate = true;

    // 상속 받은 액터가 Update에서 동작하는 경우
    protected virtual void Update()
    {
        if(CanAct())                        //동작 가능할 경우
        {
            ActorUpdate();                  //ActorUpdate 호출
        }
    }

    // 상속 받은 액터가 FixedUpdate에서 동작하는 경우
    protected virtual void FixedUpdate()
    {
        if (CanAct())                        //동작 가능할 경우
        {
            ActorFixedUpdate();             //ActorFixedUpdate 호출
        }
    }

    // 액터가 동작할 수 있는지 확인
    protected bool CanAct()
    {
        if (!canUpdate) return false;                                   //상태 변경이 가능한지 확인
        if (GameManager.Instance == null) return true;

        GameState state = GameManager.Instance.currentGameState;
        return state == GameState.Playing;                              // 게임 상태가 Playing일 경우만 동작 가능
    }

    protected virtual void ActorUpdate() { }
    protected virtual void ActorFixedUpdate() { }
}
