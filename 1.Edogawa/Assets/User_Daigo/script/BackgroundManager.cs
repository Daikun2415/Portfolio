using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] private Sprite[] backgroundSprites; // 背景用の画像を複数登録
    private SpriteRenderer spriteRenderer;

    // 指定したいサイズ（Unityのワールド座標上のサイズ）
    [SerializeField] private Vector2 targetSize = new Vector2(1980f, 1240f);

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // ランダムに背景を選択
        int index = Random.Range(0, backgroundSprites.Length);
        spriteRenderer.sprite = backgroundSprites[index];

        // 指定サイズにリサイズ
        SetBackgroundSize(targetSize);

        // 一番後ろに配置（方法① Sorting Layer）
        spriteRenderer.sortingLayerName = "Background";
        spriteRenderer.sortingOrder = -100;

        // （オプション：方法② Transform の Z 位置）
        transform.position = new Vector3(transform.position.x, transform.position.y, 10f);
    }

    private void SetBackgroundSize(Vector2 size)
    {
        if (spriteRenderer.sprite == null) return;

        // スプライト本来のサイズ（ワールド単位）
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        // スケール比率を計算
        Vector3 scale = transform.localScale;
        scale.x = size.x / spriteSize.x;
        scale.y = size.y / spriteSize.y;

        transform.localScale = scale;
    }
}
