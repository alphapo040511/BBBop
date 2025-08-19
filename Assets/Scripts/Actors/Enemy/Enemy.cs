using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 테스트용 임시 액터
public class Enemy : Actor
{
    public float speed = 2f;
    public Transform target;

    protected override void ActorUpdate()
    {
        if(target != null)
        {
            Vector3 dir = (target.position - transform.position).normalized;        // 타겟을 추적하는 코드
            transform.Translate(dir * speed * Time.deltaTime);
        }
    }
}
