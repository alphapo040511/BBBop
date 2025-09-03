using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : Actor
{
    [Header("시간 감소 설정")]
    public float critRate = 0.15f;
    public float defaultDecrease = 0.05f;
    public float critDecrease = 0.1f;

    protected override void ActorUpdate()
    {
        if(Input.GetMouseButtonDown(0))
        {
            float amount = UnityEngine.Random.value < critRate? critDecrease : defaultDecrease;

            GameEvents.ClickEvent(amount);

            GameObject ui = ObjectPool.Instance.Spawn("TimeUI", Vector3.zero, Quaternion.identity);
            ui.GetComponent<TimeUI>().Show((int)(amount * 100), Input.mousePosition);
        }
    }
}
