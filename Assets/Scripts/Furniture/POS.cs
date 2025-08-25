using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class POS : Conveyor
{
    private List<GameObject> moneyList = new List<GameObject>();

    //protected override void Move()
    //{
    //    foreach(var target in moneyList)
    //    {
    //        // 목표 위치 (컨베이어 위치 + y 오프셋)
    //        Vector3 targetPos = transform.position + Vector3.up * 0.5f;

    //        target.transform.position = Vector3.MoveTowards(
    //            target.transform.position,
    //            targetPos,
    //            Time.deltaTime
    //        );

    //        // 다 왔으면 판매
    //        if (Vector3.Distance(target.transform.position, targetPos) < 0.01f)
    //        {
    //            int money = target.GetComponent<Money>().money;
    //            Debug.Log($"{money}G 획득");
    //            ObjectPool.Instance.Despawn("Money", target);
    //            moneyList.Remove(target);
    //        }
    //    }
    //}

    public override bool Enter(GameObject money)
    {
        //moneyList.Add(money);

        int amout = money.GetComponent<Money>().money;
        Debug.Log($"{amout}G 획득");
        ObjectPool.Instance.Despawn("Money", money);
        moneyList.Remove(money); ;

        return true;
    }
}
