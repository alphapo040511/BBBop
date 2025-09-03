using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ResourceGenerator : Actor
{
    [Header("Animation Settings")]
    public float popingScale = 0.95f;
    public float popingSpeed = 3f;
    [SerializeField] private bool poping = false;

    protected FurnitureData furnitureData;


    protected PlacedFurniture furniture;
    [SerializeField] private Vector2Int GenPosition;
    [SerializeField] private Vector2Int OutPosition;
    [SerializeField] private GameObject money;
    protected GridManager gridManager;

    private float timer = 0;


    private void OnDestroy()
    {
        if(money != null)
        ObjectPool.Instance.Despawn("Money", money);

        GameEvents.OnClickEvent -= DecreaseTime;
    }

    private void Start()
    {
        GameEvents.OnClickEvent += DecreaseTime;

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

    public void Initialized(FurnitureData buildingData)
    {
        this.furnitureData = buildingData;
        timer = furnitureData.intervalTime;
    }

    protected override void ActorUpdate()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            timer = furnitureData.intervalTime;              // 생성이 안되더라도 초기화


            if (money == null)
            {
                float rotation = Random.Range(0f, 360f);

                money = ObjectPool.Instance.Spawn("Money"
                    ,new Vector3(GenPosition.x + 0.5f, 0.5f, GenPosition.y + 0.5f)
                    , Quaternion.Euler(0, rotation, 0));

                money.GetComponent<Money>().money = furnitureData.goldAmount;
            }

            if (money != null)
            {
                Conveyor conveyor = GetNextConveyor();
                if(conveyor != null)
                {
                    if(conveyor.Enter(money))
                    {
                        money = null;

                        if(!poping)
                            StartCoroutine(Pop());
                    }
                }
            }
        }
    }

    private void DecreaseTime(float amount)
    {
        timer *= (1f - amount);
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

    // 재화 생성시 POP 연출
    private IEnumerator Pop()
    {
        poping = true;

        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * popingScale;

        float t = 0;

        while(t < 1)
        {
            t += Time.deltaTime * popingSpeed;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        t = 1;
        transform.localScale = targetScale;

        while (t > 0)
        {
            t -= Time.deltaTime * popingSpeed;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        transform.localScale = originalScale;

        poping = false;
    }
}
