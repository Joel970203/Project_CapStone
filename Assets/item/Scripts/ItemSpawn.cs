using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class ItemSpawn : MonoBehaviour
{
    const float tau = Mathf.PI * 2;
    [SerializeField] float coolDown = 20f;
    [SerializeField] GameObject[] items;
    [SerializeField] float itemNumMax = 100f;
    [SerializeField] float spawnRadiusMin = 60f;
    [SerializeField] float spawnRadiusMax = 250f;
    [SerializeField] float itemRadius = 10f;
    [SerializeField] float sphereCastRad = 30f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ItemCreaterCoroutine());
    }

    // Update is called once per frame
    IEnumerator ItemCreaterCoroutine()
    {
        int coroutine_Num = 0;
        while (coroutine_Num < itemNumMax)
        {
            ItemCreater();
            yield return new WaitForSeconds(coolDown);
            coroutine_Num++;
        }
    }

    void ItemCreater()
    {
        Vector3 spawn_Pos = MakeSpawnPos();
        RaycastHit hit;
        while (Physics.SphereCast(spawn_Pos, sphereCastRad, Vector3.up, out hit, 0f))
        {
            //Debug.Log("cc");
            spawn_Pos = MakeSpawnPos();
        }
        var hitt = Physics.SphereCastAll(spawn_Pos, sphereCastRad, Vector3.up, 0f);
        SpawnItem(spawn_Pos);
    }

    Vector3 MakeSpawnPos()
    {
        Vector3 pos_Center = new Vector3(564f,51f,516f);

        float spawn_Radius = Random.Range(spawnRadiusMin, spawnRadiusMax);
        float rand_Angle = Random.Range(0, tau);
        
        //원형 지형에 맞는 Sin은 X좌표, Cos은 Z좌표 담당 각도와 반지름은 랜덤으로 주어짐
        float pos_X = pos_Center.x + spawn_Radius * Mathf.Sin(rand_Angle);
        float pos_Z = pos_Center.z + spawn_Radius * Mathf.Cos(rand_Angle);

        Vector3 spawn_Pos = new Vector3(pos_X, pos_Center.y + itemRadius, pos_Z);

        return spawn_Pos;
    }

    void SpawnItem(Vector3 spawn_Pos)
    {
        int rand_Num = Random.Range(0, items.Length);
        GameObject item = items[rand_Num];

        Instantiate(item, spawn_Pos, Quaternion.identity);
    }
}

