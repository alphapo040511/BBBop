using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MoneyExchanger : Conveyor
{
    // 교환기 설정
    public float exchangeIntervalTime = 5f;
    private float currentTime = 0f;

    // 돈 생성 관련 설정
    private Vector2Int GenPosition;
    private Vector2Int OutPosition;


    private List<GameObject> targetList = new List<GameObject>();

    private int sumMoney;

    private GameObject money;

    private void OnDestroy()
    {
        if (money != null)
            ObjectPool.Instance.Despawn("MoneyBundle", money);
    }

    private void Start()
    {
        furniture = GetComponent<PlacedFurniture>();
        gridManager = FindObjectOfType<GridManager>();
        GenPosition = furniture.Start;
        OutPosition = furniture.Start;

        Vector2Int dir = furniture.Rotation switch
        {
            0 => new Vector2Int(0, 1),
            90 => new Vector2Int(1, 0),
            180 => new Vector2Int(0, -1),
            270 => new Vector2Int(-1, 0),
            _ => new Vector2Int(0, 0)
        };

        GenPosition += dir * (furniture.Size.y - 1);        // 출구 위치에 생성
        OutPosition += dir * furniture.Size.y;              // 컨배이어 밸트 위치
    }

    protected override void ActorUpdate()
    {
        if (targetList.Count > 0)
        {
            Move();
        }

        currentTime += Time.deltaTime;
        if (currentTime >= exchangeIntervalTime)
        {
            currentTime = 0f;
            Exchange(sumMoney);
            sumMoney = 0;
        }
    }

    private void Exchange(int goldAmount)
    {
        if (money == null)
        {
            float rotation = Random.Range(0f, 360f);

            money = ObjectPool.Instance.Spawn("MoneyBundle"
                , new Vector3(GenPosition.x + 0.5f, 0.5f, GenPosition.y + 0.5f)
                , Quaternion.Euler(0, rotation, 0));

            money.GetComponent<Money>().money = goldAmount;
        }

        if (money != null)
        {
            Conveyor conveyor = GetNextConveyor();
            if (conveyor != null)
            {
                if (conveyor.Enter(money))
                {
                    money = null;
                }
            }
        }
    }

    protected override void Move()
    {

        for (int i = targetList.Count - 1; i >= 0; i--)
        {
            // 목표 위치 (컨베이어 위치 + y 오프셋)
            Vector3 targetPos = transform.position + Vector3.up * 0.5f;

            targetList[i].transform.position = Vector3.MoveTowards(
                targetList[i].transform.position,
                targetPos,
                Time.deltaTime
            );

            // 도착하면 돈 저장
            if (Vector3.Distance(targetList[i].transform.position, targetPos) < 0.01f)
            {
                sumMoney += targetList[i].GetComponent<Money>().money;
                ObjectPool.Instance.Despawn("MoneyBundle", targetList[i].gameObject);
                targetList.RemoveAt(i);
            }
        }
    }

    public override bool Enter(GameObject money)
    {
        targetList.Add(money);
        return true;
    }


    private Conveyor GetNextConveyor()
    {
        if (gridManager.GetBuildingAtPosition(OutPosition.x, OutPosition.y, out PlacedFurniture item))
        {
            Conveyor conveyor = item.GetComponent<Conveyor>();
            if (conveyor != null)
            {
                return conveyor;
            }
        }
        return null;
    }
}
