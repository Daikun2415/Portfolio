using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class NPCScript_NabeRyu : MonoBehaviour, ICharacter
{
    [Header("移動・ジャンプ設定")]
    public float moveSpeed = 3f;
    public float jumpForce = 5f;

    private Rigidbody2D rb;
    private Vector2 inputDir;
    private bool isGrounded;
    public int npcID;
    [SerializeField] private TextMeshProUGUI scoreText;
    public Sprite icon;
    public Animator myAnim;
    [SerializeField] private GameObject childKingyo;

    [Header("アイテム探索")]
    public float detectRange = 10f;
    public LayerMask itemLayer;
    public LayerMask groundLayer;

    [Header("ジャンプ設定")]
    [SerializeField] private int maxJumps = 2;
    private int jumpCount = 0;

    private Transform targetItem;

    // ICharacter インターフェース
    public string Name => $"NPC{npcID}";
    public Sprite Icon => icon;
    public int Score { get; set; }

    [Header("鬼ごっこ状態")]
    [SerializeField] private int downScore = 1;
    public bool isOni = false;
    [SerializeField] private float tagCooldown = 5f;
    private float lastTagTime = -999f;
    private Coroutine oniCoroutine;

    [Header("AI設定")]
    [SerializeField] private float inputUpdateInterval = 0.3f;
    private float lastInputUpdateTime = 0f;

    [Header("鬼ターゲット切り替え設定")]
    [SerializeField] private float targetSwitchTime = 5f;
    private float lastTargetSwitchTime = -999f;

    private Vector3 initialScale;

    // -------------------------------
    // Unity 標準イベント
    // -------------------------------
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        switch (npcID)
        {
            case 1: scoreText = GameObject.Find("NPC1ScoreText").GetComponent<TextMeshProUGUI>(); break;
            case 2: scoreText = GameObject.Find("NPC2ScoreText").GetComponent<TextMeshProUGUI>(); break;
            case 3: scoreText = GameObject.Find("NPC3ScoreText").GetComponent<TextMeshProUGUI>(); break;
        }
        Score = 0;
        scoreText.text = Score.ToString();
        myAnim = GetComponent<Animator>();
        initialScale = transform.localScale;
    }

    void Update()
    {
        if (Time.time - lastInputUpdateTime > inputUpdateInterval)
        {
            if (isOni)
                MoveTowardsRandomCharacter();
            else
                MoveTowardsItemOrEscape();

            lastInputUpdateTime = Time.time;
        }
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        // 水平方向移動
        float targetVelX = inputDir.x * moveSpeed;
        float velChange = targetVelX - rb.linearVelocity.x;
        rb.AddForce(new Vector2(velChange, 0), ForceMode2D.Impulse);

        // ジャンプ
        if (inputDir.y > 0 && jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount++;
        }
    }

    // -------------------------------
    // NPC AI（鬼のときランダムキャラ追跡）
    // -------------------------------
    private void MoveTowardsRandomCharacter()
    {
        bool needNew =
            targetItem == null ||
            targetItem.GetComponent<ICharacter>() == null ||
            Time.time - lastTargetSwitchTime > targetSwitchTime ||
            targetItem.gameObject == this.gameObject;

        if (needNew)
        {
            var candidates = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<ICharacter>()
                .Where(c => !ReferenceEquals(c, this) && (c as MonoBehaviour) != null)
                .ToList();

            if (candidates.Count > 0)
            {
                var pick = candidates[Random.Range(0, candidates.Count)];
                targetItem = (pick as MonoBehaviour).transform;
                lastTargetSwitchTime = Time.time;
            }
            else
            {
                inputDir = Vector2.zero;
                return;
            }
        }

        Vector2 dir = ((Vector2)targetItem.position - (Vector2)transform.position).normalized;
        inputDir.x = Mathf.Sign(dir.x);
        inputDir.y = (dir.y > 0.1f && isGrounded) ? 1 : 0;
    }

    // -------------------------------
    // NPC AI（アイテム取得 or 鬼回避）
    // -------------------------------
    private void MoveTowardsItemOrEscape()
    {
        FindClosestItem();
        Vector2 dir = Vector2.zero;

        if (targetItem != null)
        {
            Vector2 targetPos = targetItem.position;
            RaycastHit2D hit = Physics2D.Raycast(targetPos, Vector2.down, 2f, groundLayer);

            if (targetPos.y < transform.position.y && hit.collider != null)
            {
                dir = new Vector2(Mathf.Sign(targetPos.x - transform.position.x), -0.2f);
            }
            else
            {
                dir = (targetPos - (Vector2)transform.position).normalized;
            }
        }

        var oniList = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ICharacter>()
            .Where(c => c.IsOni() && !ReferenceEquals(c, this))
            .ToList();

        float escapeRange = 3f;
        Transform nearestOni = null;
        float minDist = float.MaxValue;

        foreach (var oni in oniList)
        {
            float dist = Vector2.Distance(transform.position, (oni as MonoBehaviour).transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestOni = (oni as MonoBehaviour).transform;
            }
        }

        if (nearestOni != null && minDist < escapeRange)
        {
            dir = ((Vector2)transform.position - (Vector2)nearestOni.position).normalized;
        }

        inputDir.x = Mathf.Sign(dir.x);
        inputDir.y = (dir.y > 0.1f && isGrounded) ? 1 : 0;
    }

    private void FindClosestItem()
    {
        if (targetItem != null && !targetItem.gameObject.activeSelf)
            targetItem = null;

        if (targetItem == null)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRange, itemLayer);
            float minDist = float.MaxValue;
            Transform newTarget = null;

            foreach (var hit in hits)
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    newTarget = hit.transform;
                }
            }
            if (newTarget != null) targetItem = newTarget;
        }
    }

    // -------------------------------
    // 衝突処理
    // -------------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Platform"))
        {
            isGrounded = true;
            jumpCount = 0;
        }

        if (collision.collider.CompareTag("Player"))
        {
            Rigidbody2D otherRb = collision.collider.GetComponent<Rigidbody2D>();
            if (otherRb != null)
            {
                float mySpeed = rb.linearVelocity.magnitude;
                float otherSpeed = otherRb.linearVelocity.magnitude;

                Rigidbody2D weaker = (mySpeed < otherSpeed) ? rb : otherRb;
                Rigidbody2D stronger = (mySpeed < otherSpeed) ? otherRb : rb;

                Vector2 dir = (weaker.position - stronger.position).normalized;
                weaker.AddForce(dir * 3f, ForceMode2D.Impulse);
            }
        }

        // 鬼交代処理（クールダウンあり）
        if (isOni)
        {
            if (Time.time - lastTagTime < tagCooldown) return;

            ICharacter target = collision.collider.GetComponent<ICharacter>();
            if (target != null && !target.IsOni())
            {
                target.SetOni(true);
                this.SetOni(false);

                lastTagTime = Time.time;
                Debug.Log($"{Name} が {target.Name} に鬼を移した！（クールダウン開始）");
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Platform"))
            isGrounded = false;
    }

    // -------------------------------
    // アニメーション更新
    // -------------------------------
    private void UpdateAnimator()
    {
        if (myAnim == null) return;

        bool isJumping = Mathf.Abs(rb.linearVelocity.y) > 0.01f;
        myAnim.SetBool("IsJump", isJumping);

        bool isRunning = !isJumping && Mathf.Abs(rb.linearVelocity.x) > 0.01f;
        myAnim.SetBool("IsRun", isRunning);

        if (rb.linearVelocity.x > 0.01f)
            transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
        else if (rb.linearVelocity.x < -0.01f)
            transform.localScale = new Vector3(-Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
    }

    // -------------------------------
    // ICharacter 実装
    // -------------------------------
    public void SetOni(bool value)
    {
        isOni = value;
        if (value)
        {
            childKingyo.SetActive(true);
            lastTagTime = Time.time;
            if (oniCoroutine == null)
                oniCoroutine = StartCoroutine(OniScoreDown());
        }
        else
        {
            childKingyo.SetActive(false);
            if (oniCoroutine != null)
            {
                StopCoroutine(oniCoroutine);
                oniCoroutine = null;
            }
        }
    }

    public bool IsOni() => isOni;

    private IEnumerator OniScoreDown()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            Score -= downScore;
            if (Score < 0) Score = 0;
            scoreText.text = Score.ToString();
        }
    }

    public void ApplyScoreEffect(float value)
    {
        Score += (int)value;
        scoreText.text = Score.ToString();
    }

    public void ApplySpeedEffect(float value, float duration)
    {
        StartCoroutine(SpeedCoroutine(value, duration));
    }

    private IEnumerator SpeedCoroutine(float value, float duration)
    {
        moveSpeed += value;
        yield return new WaitForSeconds(duration);
        moveSpeed -= value;
    }
}
