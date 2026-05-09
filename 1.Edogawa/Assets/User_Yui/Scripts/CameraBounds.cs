using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraBounds : MonoBehaviour
{
    [SerializeField] private float wallThickness = 2f; // 厚みを増やす
    [SerializeField] private float offset = 0.3f;      // キャラクターがくっつかない余裕

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        float height = cam.orthographicSize;
        float width = height * cam.aspect;

        // カメラ外側に壁を配置
        CreateWall(new Vector2(-width - wallThickness / 2 - offset, 0), new Vector2(wallThickness, height * 2)); // Left
        CreateWall(new Vector2(width + wallThickness / 2 + offset, 0), new Vector2(wallThickness, height * 2));  // Right
        CreateWall(new Vector2(0, height + wallThickness / 2 + offset), new Vector2(width * 2, wallThickness)); // Top
        // Bottomは既存オブジェクト(yuka1)を使用
    }

    void CreateWall(Vector2 pos, Vector2 size)
    {
        GameObject wall = new GameObject("Wall");
        wall.transform.parent = this.transform;
        wall.transform.localPosition = pos;

        BoxCollider2D col = wall.AddComponent<BoxCollider2D>();
        col.size = size;

        Rigidbody2D rb = wall.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
    }
}
