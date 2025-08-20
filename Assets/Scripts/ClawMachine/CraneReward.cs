using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static FurnitureData;

public class CraneReward : MonoBehaviour
{
    [SerializeField] FurnitureData[] reward;
    public Collider objectCheckCollider;

    [Header("등급별 확률")]
    [Range(0, 100)] public int commonRate = 69;
    [Range(0, 100)] public int rareRate = 20;
    [Range(0, 100)] public int epicRate = 10;
    [Range(0, 100)] public int uniqueRate = 1;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Grabbable"))
        {
            var reward = GetRandomFurniture();
            FurnitureManager.Instance.GetFurniture(reward.id);
            SaveManager.Instance.SaveData();
            Debug.Log($"{reward.id} 획득!");
        }
    }

    private FurnitureData GetRandomFurniture()
    {
        int roll = Random.Range(0, 100);                    //랜덤으로 0~100중 하나

        Probability selectedProbability;

        if (roll < commonRate)                              //roll이 일반 등급의 확률(69)보다 낮을 때
        {
            selectedProbability = Probability.Common;
        }
        else if (roll < commonRate + rareRate)              //roll이 일반 등급 + 희귀 등급 더한 값(69 + 15)보다 낮을 때
        {
            selectedProbability = Probability.Rare;
        }
        else if (roll < commonRate + rareRate + epicRate)   //roll이 일반 등급 + 희귀 등급 + 영웅 등급을 더한 값(69 + 15 + 5)보다 낮을 때
        {
            selectedProbability = Probability.Epic;
        }
        else                                                //그 외
        {
            selectedProbability = Probability.Unique;
        }

        //선택된 등급의 가구 목록
        List<FurnitureData> table = new List<FurnitureData>();
        for (int i = 0; i < reward.Length; i++)
        {
            if (reward[i].probability == selectedProbability)
            {
                table.Add(reward[i]);
            }
        }

        //선택된 목록에 해당 등급의 가구가 없을 때 일반으로 대체
        if (table.Count == 0)
        {
            for (int i = 0; i < reward.Length; i++)
            {
                if (reward[i].probability == Probability.Common)
                {
                    table.Add(reward[i]);
                }
            }
        }

        //후보중에서 랜덤으로 선택
        int randomIndex = Random.Range(0, table.Count);
        return table[randomIndex];
    }

}
