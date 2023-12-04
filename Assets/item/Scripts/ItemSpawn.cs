using System.Collections;
using UnityEngine;
using Photon.Pun;
using Random = UnityEngine.Random;

public class ItemSpawn : MonoBehaviourPunCallbacks
{
    const float Tau = Mathf.PI * 2;
    [SerializeField] float coolDown = 20f;
    [SerializeField] GameObject[] items;
    [SerializeField] float itemNumMax = 100f;
    [SerializeField] float spawnRadiusMin = 60f;
    [SerializeField] float spawnRadiusMax = 250f;
    [SerializeField] float itemRadius = 10f;
    [SerializeField] float sphereCastRad = 30f;

    bool isMasterClient = false;
    bool coroutineStarted = false;

    void Start()
    {
        isMasterClient = PhotonNetwork.IsMasterClient;
        if (isMasterClient)
        {
            StartCoroutine(ItemCreaterCoroutine());
        }
    }

    IEnumerator ItemCreaterCoroutine()
    {
        if (coroutineStarted)
        {
            yield break; // 이미 실행 중인 경우 중복 실행 방지
        }

        coroutineStarted = true;

        for (int i = 0; i < itemNumMax; i++)
        {
            Vector3 spawn_Pos = MakeSpawnPos();
            RaycastHit hit;
            while (Physics.SphereCast(spawn_Pos, sphereCastRad, Vector3.up, out hit, 0f))
            {
                spawn_Pos = MakeSpawnPos();
            }
            SpawnItem(spawn_Pos);
            yield return new WaitForSeconds(coolDown);
        }

        coroutineStarted = false;
    }

    Vector3 MakeSpawnPos()
    {
        Vector3 pos_Center = new Vector3(564f, 51f, 516f);

        float spawn_Radius = Random.Range(spawnRadiusMin, spawnRadiusMax);
        float rand_Angle = Random.Range(0, Tau);

        float pos_X = pos_Center.x + spawn_Radius * Mathf.Sin(rand_Angle);
        float pos_Z = pos_Center.z + spawn_Radius * Mathf.Cos(rand_Angle);

        Vector3 spawn_Pos = new Vector3(pos_X, pos_Center.y + itemRadius, pos_Z);

        return spawn_Pos;
    }

    void SpawnItem(Vector3 spawn_Pos)
    {
        if (PhotonNetwork.IsConnected && isMasterClient)
        {
            int rand_Num = Random.Range(0, items.Length);
            GameObject itemPrefab = items[rand_Num];
            PhotonNetwork.Instantiate(itemPrefab.name, spawn_Pos, Quaternion.identity);
        }
    }
}
