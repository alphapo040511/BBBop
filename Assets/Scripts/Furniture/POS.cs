using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class POS : Conveyor
{
    public AudioSource source;

    private Queue<Money> moneyQueue = new Queue<Money>();

    private float interval = 0.3f;
    private float timer;

    protected override void Update()
    {
        base.Update();

        if (moneyQueue.Count > 0)
        {
            timer += Time.deltaTime;

            if (timer >= interval)
            {
                timer = 0;
                ShowUI();
            }
        }
    }

    private void ShowUI()
    {
        if (moneyQueue.Count <= 0) return;

        Money gold = moneyQueue.Dequeue();

        GameObject ui = ObjectPool.Instance.Spawn("GoldUI", Vector3.zero, Quaternion.identity);

        ui.GetComponent<GoldUI>().Show(gold.money, transform);

        ObjectPool.Instance.Despawn(gold.name, gold.gameObject);

        ResourceManager.Instance.GetGold(gold.money);

        source.Play();                                                       // 매서드 나중에 정리
    }

    public override bool Enter(GameObject money)
    {
        Money gold = money.GetComponent<Money>();

        moneyQueue.Enqueue(gold);

        return true;
    }
}
