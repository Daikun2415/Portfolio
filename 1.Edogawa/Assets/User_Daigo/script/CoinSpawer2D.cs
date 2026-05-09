using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner2D : MonoBehaviour
{
    public GameObject coinPrefab;
    public float spawnInterval = 2f;

    // 空中の出現範囲
    public Vector2 spawnAreaMin;
    public Vector2 spawnAreaMax;

    // 床の高さ
    public float floorY = -3f;

    public float safeDistance = 0.2f; // 重なり防止
    public LayerMask obstacleLayers;  // コイン・床・足場のレイヤーを登録

    public int instantSpawnCount = 12; // 0個になったとき即時に出す数
    public int maxRetryPerCoin = 10000; // 1コインあたり最大リトライ数

    void Start()
    {
        InvokeRepeating(nameof(SpawnCoin), 7f, spawnInterval);
    }

    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("point").Length == 0)
        {
            ForceSpawnMultipleCoins(instantSpawnCount);
        }
    }

    void ForceSpawnMultipleCoins(int count)
    {
        int spawned = 0;

        // 必ず count 枚出るまで回す
        while (spawned < count)
        {
            Vector2 spawnPos = Vector2.zero;
            bool found = false;

            for (int i = 0; i < maxRetryPerCoin; i++)
            {
                spawnPos = GetRandomSpawnPosition();

                if (!Physics2D.OverlapCircle(spawnPos, safeDistance, obstacleLayers))
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                SpriteRenderer itemSprite = coinPrefab.GetComponent<SpriteRenderer>();
                
                Instantiate(coinPrefab, spawnPos, Quaternion.identity);
                spawned++;
            }
            else
            {
                Debug.LogWarning("コインを安全にスポーンできませんでした。spawnAreaを広げる必要があるかも。");
                // これ以上は無理なので break
                break;
            }
        }
    }

    void SpawnCoin()
    {
        Vector2 spawnPos = GetRandomSpawnPosition();

        for (int i = 0; i < maxRetryPerCoin; i++)
        {
            if (!Physics2D.OverlapCircle(spawnPos, safeDistance, obstacleLayers))
            {
                Instantiate(coinPrefab, spawnPos, Quaternion.identity);
                return;
            }
            spawnPos = GetRandomSpawnPosition();
        }

        Debug.LogWarning("通常スポーンでコインを配置できませんでした。");
    }

    Vector2 GetRandomSpawnPosition()
    {
        int choice = Random.Range(0, 3); // 0=床, 1=足場, 2=空中

        switch (choice)
        {
            case 0: return GetSpawnOnFloor();
            case 1: return GetSpawnAbovePlatform();
            default: return GetSpawnInAir();
        }
    }

    Vector2 GetSpawnOnFloor()
    {
        float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        return new Vector2(x, floorY + 1f);
    }

    Vector2 GetSpawnAbovePlatform()
    {
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
        if (platforms.Length == 0) return GetSpawnInAir();

        GameObject chosenPlatform = platforms[Random.Range(0, platforms.Length)];
        Bounds bounds = chosenPlatform.GetComponent<Collider2D>().bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = bounds.max.y + 1f;
        return new Vector2(x, y);
    }

    Vector2 GetSpawnInAir()
    {
        float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        return new Vector2(x, y);
    }
}
