using UnityEngine;

public class Coin : MonoBehaviour
{
    public int[] scoreOptions = { 5, 10, 12, 30 ,50,100};//点数のリスト
    public int[] weights = { 60, 40, 35, 20 ,10,1};//それぞれの点数の出現確率
    public int scoreValue;       // 取った時に増える点数
    public float rotateSpeed = 100f;  // 回転速度
    public float floatAmplitude = 0.1f; // 上下の幅
    public float floatFrequency = 2f;   // 上下の速さ
                              

    [Header("スコアごとの見た目")]
    public Sprite score1Sprite;
    public Sprite score2Sprite;
    public Sprite score3Sprite;
    public Sprite score10Sprite;
    public Sprite score11Sprite;
    public Sprite score12Sprite;

    private Vector2 startPos;
    private SpriteRenderer spriteRenderer;

    public AudioClip pickupSE;   // 取得時の効果音をInspectorで設定
    private AudioSource audioSource;

    void Start()
    {
        startPos = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // スコアを決定
        scoreValue = GetWeightedRandomScore();
        // スコアに応じてスプライトを変更
        spriteRenderer = GetComponent<SpriteRenderer>();
        ApplySpriteByScore(scoreValue);
    }
    private void ApplySpriteByScore(int score)
    {
        Debug.Log("決定されたスコア: " + score);
        switch (score)
        {
            case 5:
                spriteRenderer.sprite = score1Sprite;
                break;
            case 10:
                spriteRenderer.sprite = score2Sprite;
                break;
            case 12:
                spriteRenderer.sprite = score3Sprite;
                break;
            case 30:
                spriteRenderer.sprite = score10Sprite;
                break;
            case 50:
                spriteRenderer.sprite = score11Sprite;
                break;
            case 100:
                spriteRenderer.sprite = score12Sprite;
                break;
            default:
                break;
        }
        // スコアに応じてスプライトを変更
    }

    private int GetWeightedRandomScore()
    {
        int totalWeight = 0;
        foreach (int w in weights) totalWeight += w;

        int rand = Random.Range(0, totalWeight);

        int cumulative = 0;
        for (int i = 0; i < scoreOptions.Length; i++)
        {
            cumulative += weights[i];
            if (rand < cumulative)
            {
                return scoreOptions[i];
            }
        }
        return scoreOptions[0];
    }


    public int GetScore()
    {
        return scoreValue;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector2(startPos.x, newY);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ICharacter character = other.GetComponent<ICharacter>();
        if (character != null)
        {
            character.ApplyScoreEffect(scoreValue);

            SEManager.Instance.PlaySE(pickupSE);

            // アイテム本体は即削除
            Destroy(gameObject);
        }
    }

}
