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
    //        // ��ǥ ��ġ (�����̾� ��ġ + y ������)
    //        Vector3 targetPos = transform.position + Vector3.up * 0.5f;

    //        target.transform.position = Vector3.MoveTowards(
    //            target.transform.position,
    //            targetPos,
    //            Time.deltaTime
    //        );

    //        // �� ������ �Ǹ�
    //        if (Vector3.Distance(target.transform.position, targetPos) < 0.01f)
    //        {
    //            int money = target.GetComponent<Money>().money;
    //            Debug.Log($"{money}G ȹ��");
    //            ObjectPool.Instance.Despawn("Money", target);
    //            moneyList.Remove(target);
    //        }
    //    }
    //}

    public override bool Enter(GameObject money)
    {
        //moneyList.Add(money);

        int amout = money.GetComponent<Money>().money;
        Debug.Log($"{amout}G ȹ��");
        ObjectPool.Instance.Despawn("Money", money);
        moneyList.Remove(money); ;

        return true;
    }
}
