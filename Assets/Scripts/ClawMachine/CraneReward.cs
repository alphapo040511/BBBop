using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static FurnitureData;

public class CraneReward : MonoBehaviour
{
    [SerializeField] FurnitureData[] reward;
    public Collider objectCheckCollider;

    [Header("��޺� Ȯ��")]
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
            Debug.Log($"{reward.id} ȹ��!");
        }
    }

    private FurnitureData GetRandomFurniture()
    {
        int roll = Random.Range(0, 100);                    //�������� 0~100�� �ϳ�

        Probability selectedProbability;

        if (roll < commonRate)                              //roll�� �Ϲ� ����� Ȯ��(69)���� ���� ��
        {
            selectedProbability = Probability.Common;
        }
        else if (roll < commonRate + rareRate)              //roll�� �Ϲ� ��� + ��� ��� ���� ��(69 + 15)���� ���� ��
        {
            selectedProbability = Probability.Rare;
        }
        else if (roll < commonRate + rareRate + epicRate)   //roll�� �Ϲ� ��� + ��� ��� + ���� ����� ���� ��(69 + 15 + 5)���� ���� ��
        {
            selectedProbability = Probability.Epic;
        }
        else                                                //�� ��
        {
            selectedProbability = Probability.Unique;
        }

        //���õ� ����� ���� ���
        List<FurnitureData> table = new List<FurnitureData>();
        for (int i = 0; i < reward.Length; i++)
        {
            if (reward[i].probability == selectedProbability)
            {
                table.Add(reward[i]);
            }
        }

        //���õ� ��Ͽ� �ش� ����� ������ ���� �� �Ϲ����� ��ü
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

        //�ĺ��߿��� �������� ����
        int randomIndex = Random.Range(0, table.Count);
        return table[randomIndex];
    }

}
