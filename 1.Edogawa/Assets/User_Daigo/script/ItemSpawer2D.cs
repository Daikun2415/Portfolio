using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner2D : MonoBehaviour
{
    public GameObject ItemPrefab;
    public float spawnInterval = 2f;
    
    // 空中の出現範囲
    public Vector2 spawnAreaMin;
    public Vector2 spawnAreaMax;

    // 床の高さ
    public float floorY = -3f;

    public float safeDistance = 0.3f; // 重なり防止
    public LayerMask obstacleLayers;  // コイン・床・足場のレイヤーを登録

    void Start()
    {
        InvokeRepeating(nameof(SpawnItem), 7f, spawnInterval);//〇fで最初の出現時間の調整
    }

    void SpawnItem()
    {
        Vector2 spawnPos = GetRandomSpawnPosition();

        // 重なっていたらリトライ
        if (Physics2D.OverlapCircle(spawnPos, safeDistance, obstacleLayers))
        {
            SpawnItem();
            return;
        }
        
        Instantiate(ItemPrefab, spawnPos, Quaternion.identity);
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
        return new Vector2(x, floorY + 1f); // 床の上に1f浮かせる
    }

    Vector2 GetSpawnAbovePlatform()
    {
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
        if (platforms.Length == 0) return GetSpawnInAir();

        GameObject chosenPlatform = platforms[Random.Range(0, platforms.Length)];
        Bounds bounds = chosenPlatform.GetComponent<Collider2D>().bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = bounds.max.y + 1f; // 足場から1f上
        return new Vector2(x, y);
    }

    Vector2 GetSpawnInAir()
    {
        float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        return new Vector2(x, y);
    }
}
